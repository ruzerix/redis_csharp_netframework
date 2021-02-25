using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace redissss
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Console App.");

            try
            {
                IDatabase rd = redis.Connect();
                Console.WriteLine("\nConnect redis Success");
                TimeSpan ts = rd.Ping();
                Console.WriteLine($"redis Pong: {ts.ToString()}");

                //STRING
                Console.WriteLine("\n\n------- STRING -------");
                Console.WriteLine(">> set get command");
                string strKey = "AAA";
                if (rd.StringSet(strKey, "test string"))
                {
                    var val = rd.StringGet(strKey);
                    //output - test string
                    Console.WriteLine(val);
                }
                Console.WriteLine(">> mset mget command");
                KeyValuePair<RedisKey, RedisValue>[] strPair = {
                    new KeyValuePair<RedisKey, RedisValue>("A", "one"),
                    new KeyValuePair<RedisKey, RedisValue>("B", "two")
                };
                if (rd.StringSet(strPair))
                {
                    RedisKey[] myKeys = { "A", "B" };
                    var allValues = rd.StringGet(myKeys);

                    //output - one, two
                    Console.WriteLine(string.Join(",", allValues));
                }
                Console.WriteLine(">> incr incrby command");
                string intKey = "num";
                if (rd.StringSet(intKey, 100))
                {
                    //incr command
                    var result = rd.StringIncrement(intKey); //after operation Our int number is now 102
                    Console.WriteLine(result);

                    //incrby command
                    var newNumber = rd.StringIncrement(intKey, 100); // we now have incremented by 100, thus the new number is 202
                    Console.WriteLine(newNumber);
                }


                //LIST
                Console.WriteLine("\n\n------- LIST -------");
                var listKey = "listKey";
                Console.WriteLine(">> del rpush lpush llen command");
                rd.KeyDelete(listKey, CommandFlags.FireAndForget);
                rd.ListRightPush(listKey, "a");
                var llen = rd.ListLength(listKey);
                Console.WriteLine(llen); //output - 1
                rd.ListLeftPush(listKey, "b");
                Console.WriteLine(rd.ListLength(listKey)); //output - 2

                Console.WriteLine(">> lrange ltrim command");
                //lets clear it out
                rd.KeyDelete(listKey, CommandFlags.FireAndForget);
                rd.ListRightPush(listKey, "abcdefghijklmnopqrstuvwxyz".Select(x => (RedisValue)x.ToString()).ToArray());
                Console.WriteLine(rd.ListLength(listKey)); //output - 26
                Console.WriteLine(string.Concat(rd.ListRange(listKey))); //output - abcdefghijklmnopqrstuvwxyz
                var lastFive = rd.ListRange(listKey, -5);
                Console.WriteLine(string.Concat(lastFive)); //output - vwxyz
                var firstFive = rd.ListRange(listKey, 0, 4);
                Console.WriteLine(string.Concat(firstFive)); //output - abcde
                rd.ListTrim(listKey, 0, 1);
                Console.WriteLine(string.Concat(rd.ListRange(listKey))); //output - ab

                Console.WriteLine(">> lpop rpopcommand");
                //lets clear it out
                rd.KeyDelete(listKey, CommandFlags.FireAndForget);
                rd.ListRightPush(listKey, "abcdefghijklmnopqrstuvwxyz".Select(x => (RedisValue)x.ToString()).ToArray());
                var firstElement = rd.ListLeftPop(listKey);
                Console.WriteLine(firstElement); //output - a, list is now bcdefghijklmnopqrstuvwxyz
                var lastElement = rd.ListRightPop(listKey);
                Console.WriteLine(lastElement); //output - z, list is now bcdefghijklmnopqrstuvwxy

                Console.WriteLine(">> lrem lset lindex command");
                rd.ListRemove(listKey, "c");
                Console.WriteLine(string.Concat(rd.ListRange(listKey))); //output - bdefghijklmnopqrstuvwxy   
                rd.ListSetByIndex(listKey, 1, "c");
                Console.WriteLine(string.Concat(rd.ListRange(listKey))); //output - bcefghijklmnopqrstuvwxy   
                var thirdItem = rd.ListGetByIndex(listKey, 3);
                Console.WriteLine(thirdItem); //output - f  

                Console.WriteLine(">> rpoplpush command");
                //lets clear it out
                var destinationKey = "destinationList";
                rd.KeyDelete(listKey, CommandFlags.FireAndForget);
                rd.KeyDelete(destinationKey, CommandFlags.FireAndForget);
                rd.ListRightPush(listKey, "abcdefghijklmnopqrstuvwxyz".Select(x => (RedisValue)x.ToString()).ToArray());
                var listLength = rd.ListLength(listKey);
                for (var i = 0; i < listLength; i++)
                {
                    var val = rd.ListRightPopLeftPush(listKey, destinationKey);
                    Console.Write(val);    //output - zyxwvutsrqponmlkjihgfedcba
                }


                //HASH
                Console.WriteLine("\n\n------- HASH -------");
                var hashKey = "hashKey";
                HashEntry[] redisGameHash = {
                    new HashEntry("name", "Adventure in C#"),
                    new HashEntry("year", 2021),
                    new HashEntry("company", "RuzeriE.K")
                };
                Console.WriteLine(">> hset hget hexists hgetall command");
                rd.HashSet(hashKey, redisGameHash);
                if (rd.HashExists(hashKey, "year"))
                {
                    //output - 2021
                    var year = rd.HashGet(hashKey, "year");
                }
                var allHash = rd.HashGetAll(hashKey);
                //get all the items
                foreach (var item in allHash)
                {
                    //output 
                    //key: name, value: Adventure in C#
                    //key: year, value: 2021
                    //key: company, value: RuzeriE.K
                    Console.WriteLine(string.Format("key : {0}, value : {1}", item.Name, item.Value));
                }
                Console.WriteLine(">> hvals hkeys command");
                //get all the values
                var values = rd.HashValues(hashKey);
                foreach (var val in values)
                {
                    //output - Adventure in C#, 2021, RuzeriE.K
                    Console.WriteLine(val);
                }

                //get all the keys
                var keys = rd.HashKeys(hashKey);
                foreach (var k in keys)
                {
                    //output - title, year, author
                    Console.WriteLine(k); 
                }
                Console.WriteLine(">> hlen hincrby hincrbyfloat command");
                //output - 3 (len)
                Console.WriteLine($"hash len: {rd.HashLength(hashKey)}");  
                if (rd.HashExists(hashKey, "year"))
                {
                    Console.WriteLine($"year increment: {rd.HashIncrement(hashKey, "year", 1)}"); //year now becomes 2022
                    Console.WriteLine($"year decrement: {rd.HashDecrement(hashKey, "year", 1.5)}"); //year now becomes 2020.5
                }


            } catch (Exception e)
            {
                Console.WriteLine($"Exception =>  {e.Message}");
            }

            Console.WriteLine("\n\nPlease ENTER to Exit....");
            Console.ReadLine();
        }
    }
}
