using System;
using System.Collections.Generic;
using System.IO;

namespace facs
{
    public class Program
    {
        public static void Main(string[] arguments)
        {
            int bb;
            string bbText;
            bool mb;
            bool pause;

            mb = false;
            pause = false;

            if (arguments.Length > 0)
            {
                foreach (string a in arguments)
                {
                    if (a.IndexOf("-mb", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        mb = true;
                    }
                    else if (a.IndexOf("-pause", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        pause = true;
                    }
                }
            }

            if (mb)
            {
                bb = 1024 * 1024;
                bbText = "mb";
            }
            else
            {
                bb = 1024;
                bbText = "kb";
            }

            long[] sizes;
            int[] fileCounts;
            long[] fileSizeSum;
            bool foundSize;

            if (mb)
            {
                sizes = new long[] { bb, bb * 2, bb * 4, bb * 8, bb * 16, bb * 32, bb * 64, bb * 128, bb * 256, bb * 512, bb * 1024 };
            }
            else
            {
                sizes = new long[] { bb, bb * 2, bb * 4, bb * 8, bb * 16, bb * 32, bb * 64, bb * 128, bb * 256, bb * 512, bb * 1024, bb * 4096, bb * 8192, bb * 16384, bb * 32768, bb * 65536, bb * 131072, bb * 262144, bb * 524288, bb * 1048576 };
            }

            fileCounts = new int[sizes.Length + 1];
            fileSizeSum = new long[sizes.Length + 1];

            for (int i = 0; i < fileCounts.Length; i++)
            {
                fileCounts[i] = 0;
                fileSizeSum[i] = 0;
            }

            foreach (FileInfo f in new List<FileInfo>(new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.*", SearchOption.AllDirectories)))
            {
                foundSize = false;

                for (int i = 0; i < sizes.Length; i++)
                {
                    if (f.Length < sizes[i])
                    {
                        fileSizeSum[i] += f.Length;
                        fileCounts[i]++;
                        foundSize = true;
                        break;
                    }
                }

                if (!foundSize)
                {
                    fileSizeSum[fileSizeSum.Length - 1] += f.Length;
                    fileCounts[fileSizeSum.Length - 1]++;
                }
            }

            const string format = " │ {0,10:##,###} │ {1,10:##,###} │ {2,10:##,###} │ {3,16:##,##0.00} │ {4,16:##,##0.00} │";

            // 
            // Header
            // 

            Console.WriteLine();
            Console.WriteLine(" ┌─{0,-74}─┐", new string('─', 74));
            Console.WriteLine(" │ {0,-74} │", "facs.exe - displays file averages, counts, and sizes for the current ");
            Console.WriteLine(" │ {0,-74} │", "           directory and all sub-directories.");
            Console.WriteLine(" │ {0,-74} │", new string(' ', 74));
            Console.WriteLine(" │ {0,-74} │", "Directory: " + Environment.CurrentDirectory);

            Console.WriteLine(" ├─{0,10}─┬─{0,10}─┬─{1,10}─┬─{2,16}─┬─{2,16}─┤", new string('─', 10), new string('─', 10), new string('─', 16));
            Console.WriteLine(" │ {0,10} │ {1,10} │ {2,10} │ {3,16} │ {4,16} │", "from " + bbText, "to " + bbText, "num files", "total " + bbText, "average " + bbText);
            Console.WriteLine(" ├─{0,10}─┼─{0,10}─┼─{1,10}─┼─{2,16}─┼─{2,16}─┤", new string('─', 10), new string('─', 10), new string('─', 16));

            //│ ├ ┼ ┤ ─ ┬ ┴ └ ┐ ┘ █▄▌▐▀ ▓▒░ »«

            // 
            // File details
            // 

            if (fileCounts[0] > 0)
            {
                Console.WriteLine(format, " 0", sizes[0] / bb, fileCounts[0], Math.Round((decimal)fileSizeSum[0] / bb, 2), Math.Round((decimal)(fileSizeSum[0] / fileCounts[0]) / bb, 2));
            }
            else
            {
                Console.WriteLine(format, " 0", sizes[0] / bb, " ", " ", " ");
            }

            for (int i = 1; i < sizes.Length; i++)
            {
                if (fileCounts[i] > 0)
                {
                    Console.WriteLine(format, sizes[i - 1] / bb, sizes[i] / bb, fileCounts[i], Math.Round((decimal)fileSizeSum[i] / bb, 2), Math.Round((decimal)(fileSizeSum[i] / fileCounts[i]) / bb, 2));
                }
                else
                {
                    Console.WriteLine(format, sizes[i - 1] / bb, sizes[i] / bb, " ", " ", " ");
                }
            }

            if (fileCounts[fileCounts.Length - 1] > 0)
            {
                Console.WriteLine(format, sizes[sizes.Length - 1] / bb, "and up", fileCounts[fileCounts.Length - 1], Math.Round((decimal)fileSizeSum[fileSizeSum.Length - 1] / bb, 2), Math.Round((decimal)(fileSizeSum[fileSizeSum.Length - 1] / fileCounts[fileCounts.Length - 1]) / bb, 2));
            }
            else
            {
                Console.WriteLine(format, sizes[sizes.Length - 1] / bb, "and up", " ", " ", " ");
            }

            Console.WriteLine(" ├─{0,10}─┴─{0,10}─┼─{0,10}─┼─{1,16}─┼─{1,16}─┤", new string('─', 10), new string('─', 16));

            // 
            // Totals
            // 

            int fileCount;
            long sum;
            long avg;

            fileCount = 0;
            sum = 0;
            avg = 0;

            foreach (int c in fileCounts)
            {
                fileCount += c;
            }
            foreach (long c in fileSizeSum)
            {
                sum += c;
            }
            avg = sum / fileCount;

            Console.WriteLine(" │ {0,23} │ {1,10:##,###} │ {2,16:##,##0.00} │ {3,16:##,##0.00} │", "totals", fileCount, Math.Round((decimal)sum / bb, 2), Math.Round((decimal)avg / (decimal)bb, 2));
            Console.WriteLine(" └─{0,23}─┴─{1,10}─┴─{2,16}─┴─{2,16}─┘", new string('─', 23), new string('─', 10), new string('─', 16));
            Console.WriteLine();


            if (pause)
            {
                Console.ReadKey(true);
            }
        }
    }
}
