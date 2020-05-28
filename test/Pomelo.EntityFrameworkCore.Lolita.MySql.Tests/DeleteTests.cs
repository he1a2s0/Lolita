using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.Lolita.Tests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.Lolita.MySql.Tests
{
    public class DeleteTests
    {
        [Fact]
        public void delete_without_where_predicate()
        {
            using (var db = new MySqlContext())
            {
                var sql = db.Posts
                    .GenerateBulkDeleteSql();

                Assert.Equal(@"DELETE FROM `Posts`
;", sql, false, true, false);
            }
        }

        [Fact]
        public void delete_with_simple_where_predicate()
        {
            using (var db = new MySqlContext())
            {
                var sql = db.Posts
                    .Where(x => x.Id == 1)
                    .GenerateBulkDeleteSql();

                Assert.Equal(@"DELETE FROM `Posts`
WHERE `Posts`.`Id` = 1;", sql, false, true, false);
            }
        }

        [Fact]
        public void delete_with_complex_where_predicate()
        {
            using (var db = new MySqlContext())
            {
                var time = Convert.ToDateTime("2016-01-01");
                var sql = db.Users
                    .Where(x => db.Posts.Count(y => y.UserId == x.Id) == 0)
                    .Where(x => x.Role == UserRole.Member)
                    .GenerateBulkDeleteSql();

                Assert.Equal(@"DELETE FROM `Users`
WHERE ((
    SELECT COUNT(*)
    FROM `Posts` AS `p`
    WHERE `p`.`UserId` = `Users`.`Id`) = 0) AND (`Users`.`Role` = 0);", sql, false, true, false);
            }
        }

        [Fact(Skip ="Updating/Deleting for entity which has owned entity/entities is not supported")]
        public void delete_with_entity_has_owned_entity()
        {
            using var db = new MySqlContext();

            var sql = db.Customers
                .GenerateBulkDeleteSql();

            Assert.Equal(@"DELETE FROM `Customers`
;", sql, false, true, false);
        }
    }
}
