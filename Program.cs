/*!
	Copyright (C) 2010-2013 Kody Brown (kody@bricksoft.com).
	
	MIT License:
	
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to
	deal in the Software without restriction, including without limitation the
	rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
	sell copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
	DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;

namespace facs
{
	public class Program
	{
		private static long[] sizes;
		private static int[] fileCounts;
		private static long[] fileSizeSum;
		private static bool foundSize;

		public static void Main( string[] arguments )
		{
			int bb;
			string bbText;
			bool mb;
			bool pause;

			mb = false;
			pause = false;

			if (arguments.Length > 0) {
				foreach (string a in arguments) {
					if (a.IndexOf("-mb", StringComparison.CurrentCultureIgnoreCase) > -1) {
						mb = true;
					} else if (a.IndexOf("-pause", StringComparison.CurrentCultureIgnoreCase) > -1) {
						pause = true;
					}
				}
			}

			if (mb) {
				bb = 1024 * 1024;
				bbText = "mb";
			} else {
				bb = 1024;
				bbText = "kb";
			}

			if (mb) {
				sizes = new long[] { bb, bb * 2, bb * 4, bb * 8, bb * 16, bb * 32, bb * 64, bb * 128, bb * 256, bb * 512, bb * 1024 };
			} else {
				sizes = new long[] { bb, bb * 2, bb * 4, bb * 8, bb * 16, bb * 32, bb * 64, bb * 128, bb * 256, bb * 512, bb * 1024, bb * 4096, bb * 8192, bb * 16384, bb * 32768, bb * 65536, bb * 131072, bb * 262144, bb * 524288, bb * 1048576 };
			}

			fileCounts = new int[sizes.Length + 1];
			fileSizeSum = new long[sizes.Length + 1];

			for (int i = 0; i < fileCounts.Length; i++) {
				fileCounts[i] = 0;
				fileSizeSum[i] = 0;
			}

			// 
			// Calculate the folders
			// 
			calcFolders(Environment.CurrentDirectory);


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

			if (fileCounts[0] > 0) {
				Console.WriteLine(format, " 0", sizes[0] / bb, fileCounts[0], Math.Round((decimal)fileSizeSum[0] / bb, 2), Math.Round((decimal)(fileSizeSum[0] / fileCounts[0]) / bb, 2));
			} else {
				Console.WriteLine(format, " 0", sizes[0] / bb, " ", " ", " ");
			}

			for (int i = 1; i < sizes.Length; i++) {
				if (fileCounts[i] > 0) {
					Console.WriteLine(format, sizes[i - 1] / bb, sizes[i] / bb, fileCounts[i], Math.Round((decimal)fileSizeSum[i] / bb, 2), Math.Round((decimal)(fileSizeSum[i] / fileCounts[i]) / bb, 2));
				} else {
					Console.WriteLine(format, sizes[i - 1] / bb, sizes[i] / bb, " ", " ", " ");
				}
			}

			if (fileCounts[fileCounts.Length - 1] > 0) {
				Console.WriteLine(format, sizes[sizes.Length - 1] / bb, "and up", fileCounts[fileCounts.Length - 1], Math.Round((decimal)fileSizeSum[fileSizeSum.Length - 1] / bb, 2), Math.Round((decimal)(fileSizeSum[fileSizeSum.Length - 1] / fileCounts[fileCounts.Length - 1]) / bb, 2));
			} else {
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

			foreach (int c in fileCounts) {
				fileCount += c;
			}
			foreach (long c in fileSizeSum) {
				sum += c;
			}
			avg = sum / fileCount;

			Console.WriteLine(" │ {0,23} │ {1,10:##,###} │ {2,16:##,##0.00} │ {3,16:##,##0.00} │", "totals", fileCount, Math.Round((decimal)sum / bb, 2), Math.Round((decimal)avg / (decimal)bb, 2));
			Console.WriteLine(" └─{0,23}─┴─{1,10}─┴─{2,16}─┴─{2,16}─┘", new string('─', 23), new string('─', 10), new string('─', 16));
			Console.WriteLine();


			if (pause) {
				Console.ReadKey(true);
			}
		}

		private static void calcFolders( string path )
		{
			string[] list;
			DirectoryInfo d;
			FileInfo f;

			try {
				list = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
			} catch (Exception) {
				return;
			}

			foreach (string dname in list) {
				try {
					d = new DirectoryInfo(dname);
				} catch (Exception) {
					continue;
				}

				calcFolders(d.FullName);
			}

			list = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

			foreach (string fname in list) {
				foundSize = false;

				try {
					f = new FileInfo(fname);
				} catch (Exception) {
					continue;
				}

				for (int i = 0; i < sizes.Length; i++) {
					if (f.Length < sizes[i]) {
						fileSizeSum[i] += f.Length;
						fileCounts[i]++;
						foundSize = true;
						break;
					}
				}

				if (!foundSize) {
					fileSizeSum[fileSizeSum.Length - 1] += f.Length;
					fileCounts[fileSizeSum.Length - 1]++;
				}
			}

		}
	}
}
