using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExoMail.Data.Migrations
{
    public partial class Initial01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VirtualServerConfigs",
                columns: table => new
                {
                    VirtualServerConfigId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualServerConfigs", x => x.VirtualServerConfigId);
                });

            migrationBuilder.CreateTable(
                name: "DkimPolicyConfig",
                columns: table => new
                {
                    DkimPolicyConfigId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Domain = table.Column<string>(nullable: true),
                    Headers = table.Column<string>(nullable: true),
                    PrivateKey = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    Selector = table.Column<string>(nullable: true),
                    UseDkim = table.Column<bool>(nullable: false),
                    VirtualServerConfigId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DkimPolicyConfig", x => x.DkimPolicyConfigId);
                    table.ForeignKey(
                        name: "FK_DkimPolicyConfig_VirtualServerConfigs_VirtualServerConfigId",
                        column: x => x.VirtualServerConfigId,
                        principalTable: "VirtualServerConfigs",
                        principalColumn: "VirtualServerConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageRouteConfig",
                columns: table => new
                {
                    MessageRouteId = table.Column<Guid>(nullable: false),
                    Destination = table.Column<string>(nullable: true),
                    MaxClientsPerDomain = table.Column<int>(nullable: false),
                    MaxMessageRetries = table.Column<int>(nullable: false),
                    MaxMessagesPerSession = table.Column<int>(nullable: false),
                    MessageDeferralTime = table.Column<int>(nullable: false),
                    Metric = table.Column<int>(nullable: false),
                    RouteType = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    VirtualServerConfigId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRouteConfig", x => x.MessageRouteId);
                    table.ForeignKey(
                        name: "FK_MessageRouteConfig_VirtualServerConfigs_VirtualServerConfigId",
                        column: x => x.VirtualServerConfigId,
                        principalTable: "VirtualServerConfigs",
                        principalColumn: "VirtualServerConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    UserIdentityId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    MailboxPath = table.Column<string>(nullable: true),
                    MailboxSize = table.Column<long>(nullable: false),
                    MaxMailboxSize = table.Column<long>(nullable: false),
                    MaxMessageSize = table.Column<long>(nullable: false),
                    MessageCount = table.Column<long>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    VirtualServerConfigId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => x.UserIdentityId);
                    table.ForeignKey(
                        name: "FK_UserIdentities_VirtualServerConfigs_VirtualServerConfigId",
                        column: x => x.VirtualServerConfigId,
                        principalTable: "VirtualServerConfigs",
                        principalColumn: "VirtualServerConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailAddresses",
                columns: table => new
                {
                    EmailAddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    UserIdentityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddresses", x => x.EmailAddressId);
                    table.ForeignKey(
                        name: "FK_EmailAddresses_UserIdentities_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentities",
                        principalColumn: "UserIdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    FolderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    UserIdentityId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.FolderId);
                    table.ForeignKey(
                        name: "FK_Folders_UserIdentities_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentities",
                        principalColumn: "UserIdentityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DkimPolicyConfig_VirtualServerConfigId",
                table: "DkimPolicyConfig",
                column: "VirtualServerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAddresses_UserIdentityId",
                table: "EmailAddresses",
                column: "UserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserIdentityId",
                table: "Folders",
                column: "UserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRouteConfig_VirtualServerConfigId",
                table: "MessageRouteConfig",
                column: "VirtualServerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_VirtualServerConfigId",
                table: "UserIdentities",
                column: "VirtualServerConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DkimPolicyConfig");

            migrationBuilder.DropTable(
                name: "EmailAddresses");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "MessageRouteConfig");

            migrationBuilder.DropTable(
                name: "UserIdentities");

            migrationBuilder.DropTable(
                name: "VirtualServerConfigs");
        }
    }
}
