# Extension

Classes usadas para extender funcionalidades nativas do C#.

Objetivo e facilizar a utlização criando reutilização destas classes.

## LinqExtension

Habilita uma condicional para fazer um filtro em um query ou seja, caso uma determinada condição seja atendida, efetua um determinado filtro.

ex.:
_context.users.WhereIf( !string.isNullOrEmpty(userName), c=>c.name == userName )