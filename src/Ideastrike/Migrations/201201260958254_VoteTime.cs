using System;
using System.Data.Entity.Migrations;

namespace Ideastrike.Migrations
{
    public partial class VoteTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("Votes", "Time", c => c.DateTime(nullable:false, defaultValue:DateTime.UtcNow));
        }
        
        public override void Down()
        {
            DropColumn("Votes", "Time");
        }
    }
}
