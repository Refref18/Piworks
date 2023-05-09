using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a dictionary to store the distinct song play counts per user
            var general = new Dictionary<string, Dictionary<string, int>>();
            var table = new Dictionary<int, int>();
            var dateToFilter = DateTime.ParseExact("10/08/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;

            using (var file = new StreamReader("exhibitA-input.csv"))
            {
                file.ReadLine(); // skip the header row

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var row = line.Split('\t');
                    DateTime playTs;
                    if (!DateTime.TryParseExact(row[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out playTs))
                    {
                        if (!DateTime.TryParseExact(row[3], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out playTs))
                        {
                            continue;
                        }
                    }
                    if (playTs.Date != dateToFilter)
                    {
                        continue;
                    }

                    var songId = row[1];
                    var clientId = row[2];
                    if (!general.ContainsKey(clientId))
                    {
                        general[clientId] = new Dictionary<string, int>();
                    }
                    if (!general[clientId].ContainsKey(songId))
                    {
                        general[clientId][songId] = 0;
                    }
                    general[clientId][songId]++;
                }
            }

            foreach (var client in general)
            {
                if (!table.ContainsKey(client.Value.Count))
                {
                    table[client.Value.Count] = 0;
                }

                table[client.Value.Count]++;
            }

            using (var file = new StreamWriter("with_cs.csv"))
            {
                file.WriteLine("DISTINCT_PLAY_COUNT,CLIENT_COUNT");
                foreach (var kvp in table)
                {
                    file.WriteLine($"{kvp.Key},{kvp.Value}");
                }
            }
        }
    }
}
