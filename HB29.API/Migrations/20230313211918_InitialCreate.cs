using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hb29.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    TableName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecordId = table.Column<long>(type: "INTEGER", nullable: false),
                    NewValue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivacyPolicies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrivacyPolicyStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    conditions = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AdGroupId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Upn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PreferenceId = table.Column<long>(type: "INTEGER", nullable: false),
                    Value = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Preferences_PreferenceId",
                        column: x => x.PreferenceId,
                        principalTable: "Preferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTerms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    PrivacyPolicyId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTerms_PrivacyPolicies_PrivacyPolicyId",
                        column: x => x.PrivacyPolicyId,
                        principalTable: "PrivacyPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionProfile",
                columns: table => new
                {
                    PermissionsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ProfilesId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionProfile", x => new { x.PermissionsId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_PermissionProfile_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionProfile_Profiles_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ConcurrencyToken", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 103L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "START_ACTIVITY", "START_ACTIVITY", null, null },
                    { 104L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "DOWNLOAD_FILE_ACTIVITY", "DOWNLOAD_FILE_ACTIVITY", null, null },
                    { 105L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "UPDATE_CLUSTER", "UPDATE_CLUSTER", null, null },
                    { 106L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "SAVE_CLUSTER", "SAVE_CLUSTER", null, null },
                    { 107L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "REMOVE_CLUSTER", "REMOVE_CLUSTER", null, null },
                    { 108L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "UPDATE_COUNTRY", "UPDATE_COUNTRY", null, null },
                    { 109L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, "SAVE_COUNTRY", "SAVE_COUNTRY", null, null }
                });

            migrationBuilder.InsertData(
                table: "Preferences",
                columns: new[] { "Id", "ConcurrencyToken", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsEnabled", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, null, false, "Send e-mail on Workflow Status Change", null, null },
                    { 2L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, null, false, "Send e-mail on Node Template processed", null, null },
                    { 3L, new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, null, false, "Send e-mail on Workflow Design Expiration.", null, null }
                });

            migrationBuilder.InsertData(
                table: "ServiceSettings",
                columns: new[] { "Id", "ConcurrencyToken", "Name", "Type", "Value" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), "Configuração 1", "Int32", "30" },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), "Configuração 2", "String", "" },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), "Configuração 3", "String", "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EventDate_TableName_RecordId",
                table: "AuditLogs",
                columns: new[] { "EventDate", "TableName", "RecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionProfile_ProfilesId",
                table: "PermissionProfile",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name_Unique",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AddGroupName_Unique",
                table: "Profiles",
                column: "AdGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Name_Unique",
                table: "Profiles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_PreferenceId",
                table: "UserPreferences",
                column: "PreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTerms_PrivacyPolicyId",
                table: "UserTerms",
                column: "PrivacyPolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "PermissionProfile");

            migrationBuilder.DropTable(
                name: "ServiceSettings");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "UserTerms");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropTable(
                name: "PrivacyPolicies");
        }
    }
}
