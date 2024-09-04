using System;
using System.Collections.Generic;
using System.Configuration; // Namespace for ConfigurationManager
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks; // Namespace for Task
using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using hb29.Shared.DTO;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace hb29.Shared.Services
{
    public class AzureQueueStorage
    {
        private readonly ILogger<AzureQueueStorage> _logger;
        private readonly IConfiguration _configuration;
        private ServiceBusSender sender;
        private ServiceBusClient _queueClient;
        private List<ServiceBusProcessor> processors;

        //private string QueueName { get; set; }
        private string ConnectionString
        {
            get { return _configuration["ConnectionStrings-ServiceBus"]; }
        }
       
        private ServiceBusClient QueueClient
        {
            get
            {
                if (_queueClient == null)
                {
                    ServiceBusRetryOptions optionRetry = new ServiceBusRetryOptions()
                    {
                        TryTimeout = TimeSpan.FromSeconds(120)
                    };
                    ServiceBusClientOptions clientOptions = new ServiceBusClientOptions()
                    {
                        TransportType = ServiceBusTransportType.AmqpWebSockets,
                        RetryOptions = optionRetry
                    };

                    _queueClient = new ServiceBusClient(this.ConnectionString, clientOptions);
                }

                return _queueClient;
            }
        }
        public AzureQueueStorage(IConfiguration configuration, ILogger<AzureQueueStorage> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InsertMessage(string message, string QueueName)
        {
            sender = this.QueueClient.CreateSender(QueueName);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
        }

        public async Task<IList<NodeTemplateQueue>> GetQueue(string QueueName)
        {
            List<NodeTemplateQueue> Queue = new List<NodeTemplateQueue>();
            ServiceBusReceiver receiver = QueueClient.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });
            IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 2);

            if (receivedMessages.Count > 0)
            {
                foreach (ServiceBusReceivedMessage receivedMessage in receivedMessages)
                {
                    // get the message body as a string
                    var nodeData = receivedMessage.Body.ToObjectFromJson<NodeTemplateQueue>();

                    nodeData.QueueMessageId = receivedMessage.MessageId;
                    nodeData.QueueSequenceNumber = receivedMessage.SequenceNumber;
                    

                    Queue.Add(nodeData);
                }
            }
            
            return Queue;
        }

        public async Task<ServiceBusReceivedMessage[]> DequeueMessages(string QueueName)
        {
            //TODO: manage timeout visibility
            ServiceBusReceiver receiver = this.QueueClient.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });
            IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 20, maxWaitTime: TimeSpan.FromSeconds(5));
            
            // Get the next messages
            return receivedMessages.ToArray();
        }

        public ServiceBusReceivedMessage[] DequeueMessages(string QueueName, out ServiceBusReceiver receiver, ServiceBusReceiveMode ReceiveMode = ServiceBusReceiveMode.PeekLock)
        {
            //TODO: manage timeout visibility
            receiver = this.QueueClient.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ReceiveMode });
            IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = receiver.ReceiveMessagesAsync(maxMessages: 20, maxWaitTime: TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            // Get the next messages
            return receivedMessages.ToArray();
        }

        public void ConfigureProcessorEvent(string QueueName, Func<ProcessMessageEventArgs, Task> functionProcessMessage, Func<ProcessErrorEventArgs, Task> functionProcessError, CancellationToken stoppingToken)
        {
            if (processors == null)
                processors = new();

            if (processors.Any(c => c.EntityPath == QueueName) == false)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now} listening to the queue {QueueName}");

                var processor = this.QueueClient.CreateProcessor(QueueName, new ServiceBusProcessorOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });
                processor.ProcessMessageAsync += functionProcessMessage;
                processor.ProcessErrorAsync += functionProcessError;
                processor.StartProcessingAsync(stoppingToken);
                processors.Add(processor);  
            }
        }

        public async Task DeleteMessage(ServiceBusReceivedMessage message, string QueueName)
        {
            // Delete the message
            ServiceBusReceiver receiver = this.QueueClient.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });
            //IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 20, maxWaitTime: TimeSpan.FromSeconds(5));
            await receiver.CompleteMessageAsync(message);
        }

        public void DeleteMessage(ServiceBusReceivedMessage message, string QueueName, ref ServiceBusReceiver receiver)
        {
            // Delete the message
            if(receiver == null)
                receiver = this.QueueClient.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

            receiver.CompleteMessageAsync(message).GetAwaiter().GetResult();
        }

        public async Task<Azure.Response> DeleteMessageByParams(string MessageId, long SequenceNumber, string QueueName)
        {
            try
            {
                var messages = await DequeueMessages(QueueName);

                foreach (var message in messages)
                {
                    if (message.MessageId == MessageId && message.SequenceNumber == SequenceNumber)
                    {
                        await DeleteMessage(message, QueueName);
                        break;
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }

            return null;

        }

        public async Task SendRequestMessage(string msg, string sessionId, string fristQueueName)
        {
            var sender = QueueClient.CreateSender(fristQueueName);
            var requestMsg = new ServiceBusMessage(msg);
            requestMsg.SessionId = sessionId;
            await sender.SendMessageAsync(requestMsg);
        }

        public async Task<string> ReceiveReplyMessage(string sessionId, string secondQueueName)
        {
            var receiver = await _queueClient.AcceptSessionAsync(secondQueueName, sessionId, new ServiceBusSessionReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete});
            var replyMsg = await receiver.ReceiveMessageAsync();
            if (replyMsg != null)
            {
                return replyMsg.Body.ToString();
            }
            else
            {
                throw new Exception("Failed to get reply from server");
            }
        }

        public async Task SendReplyMessage(string msg, string sessionId, string secondQueueName)
        {
            var sender = _queueClient.CreateSender(secondQueueName);
            var requestMsg = new ServiceBusMessage(msg);
            requestMsg.SessionId = sessionId;
            await sender.SendMessageAsync(requestMsg);
        }
    }
}
