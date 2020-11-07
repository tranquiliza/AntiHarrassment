using AntiHarassment.Sql;
using System.Threading.Tasks;

namespace AntiHarassment.Tool
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string connString = "Data Source=localhost\\sqlexpress;Initial Catalog=AntiHarassment.Development;Integrated Security=True";

            var repository = new ChatRepository(connString, null);

            await repository.MigrateData().ConfigureAwait(false);


            //var repository = new SuspensionRepository(connString, null);

            //var cleanupService = new SuspensionCleanupService(repository);

            //await cleanupService.CleanupOops().ConfigureAwait(false);

            //var tagRepo = new TagRepository(connString, null);

            //var banService = new BotBanService(repository, tagRepo);

            //var users = UsernamesToBan();

            //await banService.CreateBansFor(users, "").ConfigureAwait(false);
        }

        //private static List<string> UsernamesToBan()
        //{
        //    return new List<string>();
        //}
    }
}
