using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace graphs
{
    class Program
    {
        static Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
        static Dictionary<int, bool> visited = new Dictionary<int, bool>();
        static List<List<int>> components = new List<List<int>>();
        static Regex regex = new Regex(@"\d*\d");
        const string path = "graph.txt";


        static int findNotVisited() {
            foreach (var temp in visited) {
                if (temp.Value == false)
                {
                    return temp.Key;
                }
            }
            return -1;
        }


        static void connectedComponents() {
            int current = 0;                                                    //Текущая вершина
            foreach (var j in graph)
            {
                current = j.Key;
                visited[j.Key] = false;
            }                                                                  //Заполняем массив непосещённых вершин
            int n = visited.Count;
            var vector = new Stack<int>();                                      //Массив обхода вершин                                       
            vector.Push(current);
            var temp = new List<int>();
            while (n > 0) {
                if (vector.Count != 0)                                        
                {
                    current = vector.Pop();
                } else {
                    current = findNotVisited();
                    components.Add(temp);
                    temp = new List<int>();
                }
                temp.Add(current);
                visited[current] = true;
                n--;
                foreach (var i in graph[current]) {
                    if (!visited[i] && !vector.Contains(i)) {
                        vector.Push(i);
                    }
                }
            }
            components.Add(temp);
        }


        static void readData(string path) {
            foreach (string line in File.ReadLines(path))
            {
                parse(line);
            }
        }
        
        
        static void parse(string text) {
            MatchCollection matches = regex.Matches(text);
            if (matches.Count > 0)
            {
                int a = Int32.Parse(matches[0].Value);
                int b = Int32.Parse(matches[1].Value);
                add(a, b);
                add(b, a);
            }
        }
        
        
        static void add(int a, int b)
        {
            if (graph.ContainsKey(a)) {
                if (!graph[a].Contains(b)) { graph[a].Add(b); }
            } else {
                var temp = new List<int>();
                temp.Add(b);
                graph.Add(a, temp);
            }
        }
        static int min(int a, int b)
        {
            if (a < b)
            {
                return a;
            }
            return b;
        }
        static int max(int a, int b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }
        static int Check(bool[] visited, int n) {
            var f = -1;
            for (int i = 0; i < n; i++) {
                if (!visited[i]) {
                    f = i;
                    break; }
            }
            return f;
        }

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            readData(path);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine(elapsedTime + " read data");


            stopWatch.Reset();
            stopWatch.Start();
            int vertex = graph.Count;
            int edge = 0;
            foreach (var temp in graph) {
                edge += temp.Value.Count;
            }
            edge /= 2;
            double p = 2 * edge / ((double)vertex * (vertex - 1));
            stopWatch.Stop();
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            ts = stopWatch.Elapsed;
            Console.WriteLine(elapsedTime + " rebra vershins");
            Console.WriteLine(vertex + " " + edge + " " + edge/2 + " " + p);





            stopWatch.Reset();
            stopWatch.Start();
            connectedComponents();
            int a = 0;
            foreach (var temp in components) {
                a += temp.Count;
            }
            Console.WriteLine(vertex + " " + a);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine(elapsedTime + " components");

            /*
            var e = new Dictionary<int, int>();
            var v = new List<int>();
            var vd = new Dictionary<int, int>();

            string text;
            using (StreamReader reader = new StreamReader(path))
            {
                text = reader.ReadToEnd();
            }
            MatchCollection matches = regex.Matches(text);
            if (matches.Count > 0)
            {
                int temp = 0;
                int s = 0;
                foreach (Match match in matches)
                {
                    temp = Int32.Parse(match.Value);
                    if (!v.Contains(temp))
                    {
                        v.Add(temp);
                        vd.Add(temp, s++);
                    }
                }
            }
            int n = v.Count;
            Console.WriteLine(n + " Число вершин");


            bool[,] matrix = new bool[n, n];
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    matrix[i, j] = false;
                }
            }
            bool f = true;
            int temp1 = 0;
            int temp2 = 0;
            int t = 0;
            foreach (Match match in matches)
            {
                if (f)
                {
                    temp1 = Int32.Parse(match.Value);
                    f = !f;
                } else
                {
                    temp2 = Int32.Parse(match.Value);
                    f = !f;
                }
                if (f) {
                    matrix[vd[temp1], vd[temp2]] = true;
                    matrix[vd[temp2], vd[temp1]] = true;
                    t += 1;
                }
            }
            Console.WriteLine(t + " Число рёбер");
            Console.WriteLine((double)(2*t)/(n*(n-1)) + " Плотность");
            var visited = new bool[n];
            for (int i = 0; i < n; i++) {
                visited[i] = false;
            }
            int k = 0;
            int max = 0;
            int contains = -1;
            while (Check(visited, n) != -1) {
                k++;
                int start = Check(visited, n);
                var ver = new List<int>();
                ver.Add(start);
                int r = 1;
                while (ver.Count != 0) {
                    int current = ver[0];
                    visited[current] = true;
                    for (int i = 0; i < n; i++)
                    {
                        if (matrix[current, i]) {
                            if (!visited[i] && !ver.Contains(i)) {
                                ver.Add(i);
                                r++;
                            }
                        }
                    }
                    ver.Remove(current);
                }
                if (r > max) {
                    max = r;
                    contains = start;
                }
            }
            Console.WriteLine(k + " Число компонент связности\n" + max + " Количество вершин в наибольшей компоненте");

            /*
            var ver1 = new List<int>();
            ver1.Add(contains);
            var vK = new List<int>();
            for (int i = 0; i < n; i++)
            {
                visited[i] = false;
            }
            t = 0;
            var vdK = new Dictionary<int, int>();
            while (ver1.Count != 0)
            {
                int current = ver1[0];
                visited[current] = true;
                vK.Add(v[current]);
                vdK.Add(v[current], t++);
                for (int i = 0; i < n; i++)
                {
                    if (matrix[current, i])
                    {
                        if (!visited[i] && !ver1.Contains(i))
                        {
                            ver1.Add(i);
                        }
                    }
                }
                ver1.Remove(current);
            }

            int[,] matrix1 = new int[max, max];
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    matrix1[i, j] = 999999999;
                }
            }
            f = true;
            temp1 = 0;
            temp2 = 0;
            foreach (Match match in matches)
            {
                if (f)
                {
                    temp1 = Int32.Parse(match.Value);
                    f = !f;
                }
                else
                {
                    temp2 = Int32.Parse(match.Value);
                    f = !f;
                }
                if (f)
                {
                    if (vK.Contains(temp1))
                    {
                        matrix1[vdK[temp1], vdK[temp2]] = 1;
                        matrix1[vdK[temp2], vdK[temp1]] = 1;
                    }
                }
            }
            Console.WriteLine(1);
            var E = new int[max];
            for (int i = 0; i < max; i++) {
                E[i] = 0;
            }
            for (int z = 0; z < max; z++)
            {
                for (int j = 0; j < max; j++)
                {
                    for (int i = 0; i < max; i++)
                    {
                        matrix1[i, j] = min(matrix1[i, j], matrix1[i, z] + matrix1[z, j]);
                    }
                }
                Console.WriteLine(z);
            }
            Console.WriteLine(2);
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    E[i] = Max(E[i], matrix1[i, j]);
                }
            }
            Console.WriteLine(3);
            int rad = 999999999;
            int diam = -1;
            for (int i = 0; i < n; i++)
            {
                rad = min(rad, E[i]);
                diam = Max(diam, E[i]);
            }
            Console.WriteLine(4);
            Console.WriteLine(rad + " RADIUS\n" + diam + " DIAMETR");
            */
            /*
              var allComp = new Dictionary<int, int>();
              for (int i = 0; i < n; i++)
              {
                  visited[i] = false;
              }
              while (Check(visited, n) != -1)
              {
                  int start = Check(visited, n);
                  var ver = new List<int>();
                  ver.Add(start);
                  int r = 1;
                  while (ver.Count != 0)
                  {
                      int current = ver[0];
                      visited[current] = true;
                      for (int i = 0; i < n; i++)
                      {
                          if (matrix[current, i])
                          {
                              if (!visited[i] && !ver.Contains(i))
                              {
                                  ver.Add(i);
                                  r++;
                              }
                          }
                      }
                      ver.Remove(current);
                  }

                      allComp.Add(start, r);
              }
              t = 0;
              foreach (var a in allComp) {
                  if (a.Value == 3) {
                      t++;
                  }
                  Console.WriteLine(a.Key + " " + a.Value);
              }
              Console.WriteLine(t);*/
        }
    }
}
