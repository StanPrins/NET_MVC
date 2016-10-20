namespace ChatWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        user_id = c.String(),
                        user_name = c.String(),
                        pwd = c.String(),
                        user_email = c.String(),
                        user_type = c.Boolean(nullable: false),
                        phone_number = c.Long(nullable: false),
                        online_state = c.Boolean(nullable: false),
                        reg_date = c.Long(nullable: false),
                        contact_list = c.String(),
                        room_list = c.String(),
                        auth_token = c.String(),
                        last_logdate = c.Long(nullable: false),
                        device_id = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        message_id = c.String(),
                        room_id = c.String(),
                        sender_id = c.String(),
                        receiver_id = c.String(),
                        sender_date = c.Long(nullable: false),
                        receiver_date = c.Long(nullable: false),
                        context = c.String(),
                        read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        room_id = c.String(),
                        room_name = c.String(),
                        owner_id = c.String(),
                        member_list = c.String(),
                        make_date = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Rooms");
            DropTable("dbo.Messages");
            DropTable("dbo.ChatUsers");
        }
    }
}
