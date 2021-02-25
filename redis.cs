using System;
using StackExchange.Redis;

namespace redissss
{
    class redis
    {
        private static readonly string server = "localhost";
        private static readonly Int32 port = 6379;
        private static readonly Int32 db = 0; // default redis database index


        //https://stackoverflow.com/questions/43359695/how-to-read-multiple-sets-stored-on-redis-using-some-command-or-lua-script/43433268#43433268
        //https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        private static string strConnect
        {
            get
            {
                //string redisConnection = "localhost:6379,ssl=false,allowAdmin=true,ConnectRetry=3,ConnectTimeout=5000,defaultDatabase=1";
                int timeOutSec = 30;

                string redisConnection = $"{server}:{port},syncTimeout={1000 * timeOutSec}";
                return redisConnection;
            }
        }
        public static IDatabase connection = null;
        public static IDatabase Connect()
        {
            //IDatabase connection = null;
            if (connection == null)
            {
                try
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(strConnect);
                    connection = redis.GetDatabase(db);
                    return connection;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
            else
            {
                return connection;
            }
        }

        public static void FlushAll()
        {
            //https://stackoverflow.com/questions/35452081/flush-empty-db-in-stackexchange-redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(strConnect);
            var server = redis.GetServer(strConnect);
            server.FlushDatabase();
        }
    }
}
