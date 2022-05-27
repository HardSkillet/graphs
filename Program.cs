using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace graphs
{
    public class point
    {
        public bool visited;
        public int deep;
    }
    class Program
    {
        static StringBuilder text = new StringBuilder();

        static Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();

        static List<List<int>> components = new List<List<int>>();


        static List<int> list = new List<int>();
        static Queue<int> queue = new Queue<int>();
        static Regex regex = new Regex(@"\d*\d");
        static int[,] matrix = new int[500, 500];
        static List<int> eccentricitiesAnswer = new List<int>();
        static List<bool> unvisited = new List<bool>();
        static Dictionary<int, int> func = new Dictionary<int, int>();
        const string path = "graph.txt";
        static List<int> randomDelete = new List<int>();
        static List<int> largeDelete = new List<int>();
        static Dictionary<int, int> dict = new Dictionary<int, int>();
        


        static void find(int current, int numberOfMax, Queue<int> vertex, Dictionary<int, bool> dict, Dictionary<int, point> ty)
        {
            var count = vertex.Count;
            Dictionary<int, point> unvisited = new Dictionary<int, point>(ty);
            var queue = new Queue<int>();
            queue.Enqueue(current);
            while (queue.Count > 0 && count > 0)
            {
                var temp = queue.Dequeue();
                foreach (var i in graph[temp])
                {
                    if (unvisited[i].visited)
                    {
                        unvisited[i].deep = unvisited[temp].deep + 1;
                        unvisited[i].visited = false;
                        queue.Enqueue(i);
                        if (dict[i])
                        {
                            count--;
                        }
                    }

                }
            }
            foreach (var a in vertex)
            {
                matrix[func[current], func[a]] = unvisited[a].deep;
                matrix[func[a], func[current]] = unvisited[a].deep;
            }
        }


        static List<int> randomNumberGeneration(int numberOfmax)
        {

            var max = components[numberOfmax].Count;
            var rnd = new Random();
            int n = max - 1;
            var numbers = new List<int>();
            int temp = rnd.Next(0, n);

            for (int i = 0; i < 500; i++)
            {
                while (numbers.Contains(temp))
                {
                    temp = rnd.Next(0, n);
                }
                numbers.Add(temp);
            }
            for (int i = 0; i < 500; i++)
            {
                numbers[i] = components[numberOfmax][numbers[i]];
            }
            return numbers;
        }

        static void connectedComponents()
        {
            Dictionary<int, bool> unvisited = new Dictionary<int, bool>();
            var all = new List<int>();
            int start;
            foreach (var i in graph)
            {
                unvisited.Add(i.Key, true);
                all.Add(i.Key);
            }
            var stack = new Stack<int>();
            var temp = new List<int>();
            while (all.Count > 0)
            {
                start = all[0];
                unvisited[start] = false;
                temp = new List<int>();
                temp.Add(start);
                stack.Push(start);

                while (stack.Count > 0)
                {
                    int v = stack.Pop();
                    foreach (var a in graph[v])
                    {
                        if (unvisited[a])
                        {
                            unvisited[a] = false;
                            stack.Push(a);
                            temp.Add(a);
                        }
                    }
                }
                components.Add(temp);
                all = all.Except(temp).ToList();
            }
        }


        static void readData(string path)
        {
            foreach (string line in File.ReadLines(path))
            {
                parse(line);
            }
        }
        static void parse(string text1)
        {
            MatchCollection matches = regex.Matches(text1);
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

            if (graph.ContainsKey(a))
            {
                if (!graph[a].Contains(b)) { graph[a].Add(b); }
            }
            else
            {
                var temp = new List<int>();
                temp.Add(b);
                graph.Add(a, temp);
            }
        }



        static long numberOfTriangles()
        {

            var marked_a = new Dictionary<int, bool>();
            foreach (var a in graph)
            {
                marked_a.Add(a.Key, false);
            }

            long triangles = 0;

            foreach (var a in graph)
            {
                var marked_b = new Dictionary<int, bool>();
                foreach (var c in graph[a.Key])
                {
                    marked_b.Add(c, false);
                }
                marked_b.Add(a.Key, true);
                foreach (var b in graph[a.Key])
                {
                    if (!marked_a[b])
                    {
                        foreach (var c in graph[b])
                        {
                            if (!marked_a[c] && marked_b.ContainsKey(c) && !marked_b[c]) { triangles++; }
                        }
                        marked_b[b] = true;
                    }
                }
                marked_a[a.Key] = true;
            }
            return triangles;
        }



        static void matrixConstruction(int numberOfMax)
        {
            Dictionary<int, point> unvisited = new Dictionary<int, point>();
            foreach (var i in components[numberOfMax])
            {
                var temp = new point();
                temp.visited = true;
                temp.deep = 0;
                unvisited.Add(i, temp);
            }
            var max = components[numberOfMax].Count;
            func = new Dictionary<int, int>();
            list = randomNumberGeneration(numberOfMax);
            var dict = new Dictionary<int, bool>();
            foreach (var a in graph)
            {
                dict.Add(a.Key, false);
            }
            foreach (var a in list)
            {
                dict[a] = true;
            }
            for (int i = 0; i < 500; i++)
            {
                func.Add(list[i], i);
                queue.Enqueue(list[i]);
            }
            for (int i = 0; i < 499; i++)
            {
                var j = queue.Dequeue();
                dict[j] = false;
                find(j, numberOfMax, queue, dict, unvisited);
            }

        }
        static void deleteX(int x)
        {
            var list = new Dictionary<int, int>();
            int step = (int)((double)graph.Count * x / 100);
            var unvisited = new Dictionary<int, bool>();
            int j = 0;

            foreach (var a in graph)
            {
                list.Add(j, a.Key);
                j++;
                unvisited.Add(a.Key, true);
            }

            Random rnd = new Random();
            int n = list.Count - 1;
            for (int i = 0; i < step; i++)
            {
                var temp = list[rnd.Next(0, n)];
                while (!unvisited[temp])
                {
                    temp = list[rnd.Next(0, n)];
                }
                unvisited[temp] = false;
            }
            randomDelete.Add(connectedComponents(unvisited));
        }

        static int connectedComponents(Dictionary<int, bool> un)
        {
            Dictionary<int, bool> unvisited = new Dictionary<int, bool>(un);
            var all = new List<int>();
            int start;
            int max = 0;
            foreach (var i in graph)
            {
                if (unvisited[i.Key])
                {
                    all.Add(i.Key);
                }
            }
            var stack = new Stack<int>();
            var temp = new List<int>();
            int j = 0;
            while (all.Count > j)
            {
                while (j < all.Count && !unvisited[all[j]]) { j++; }
                if (j >= all.Count) {
                    return max;
                }
                start = all[j];
                unvisited[start] = false;
                temp = new List<int>();
                temp.Add(start);
                stack.Push(start);

                while (stack.Count > 0)
                {
                    int v = stack.Pop();
                    foreach (var a in graph[v])
                    {
                        if (unvisited[a])
                        {
                            unvisited[a] = false;
                            stack.Push(a);

                            temp.Add(a);

                        }
                    }
                }
                if (max < temp.Count)
                {
                    max = temp.Count;

                }
                if (max * 2 > all.Count)
                {
                    return max;
                }
            }
            return max;
        }
        static void deleteLargeX()
        {
            var sortedDict = from entry in dict orderby entry.Key descending select entry;
            for (int i = 1; i <= 99; i++) {
                var unvisited = new Dictionary<int, bool>();
                foreach (var a in graph)
                { 
                    unvisited.Add(a.Key, true);
                }
                int step = (int)((double)graph.Count * i / 100);
                int temp = 0;
                foreach (var a in sortedDict) {
                    temp += a.Key;
                    if (temp >= step) {
                        temp = a.Value;
                        break;
                    }
                }
                foreach (var a in graph) {
                    if (a.Value.Count >= temp) {
                        unvisited[a.Key] = false;
                        step--;
                    }
                    if (step == 0) {
                        break;
                    }
                }
                largeDelete.Add(connectedComponents(unvisited));
                Console.WriteLine(i);
            }

        }


        static void Main(string[] args)
        {

            readData(path);

            int vertex = graph.Count;
            int edge = 0;
            int max1 = 0;
            int min1 = 9999999;
            int sum1 = 0;
            foreach (var temp in graph)
            {
                if (temp.Value.Count > max1) {
                    max1 = temp.Value.Count;
                }
                if (temp.Value.Count < min1) {
                    min1 = temp.Value.Count;
                }
                sum1 += temp.Value.Count;
                edge += temp.Value.Count;
            }
            edge /= 2;
            double r = (double)sum1 / vertex;


            double p = 2 * edge / ((double)vertex * (vertex - 1));


            Console.WriteLine(vertex + " - Число вершин в графе");
            Console.WriteLine(edge + " - Число рёбер в графе");
            Console.WriteLine(p + " - Плотность в графе");



            connectedComponents();

            int max = 0;
            int numberOfmax = 0;
            for (int i = 0; i < components.Count; i++)
            {

                if (components[i].Count > max)
                {
                    max = components[i].Count;
                    numberOfmax = i;
                }
            }


            Console.WriteLine(components.Count + " - Число компоннет слабой связности");
            Console.WriteLine((double)max / vertex * 100 + " - Доля вершин в максимальной по мощности компоненте");


            foreach (var a in largeDelete)
            {
                Console.WriteLine((double)a * 100 / vertex);
            }

            matrixConstruction(numberOfmax);

            var eccentricities = new List<int>();
            for (int i = 0; i < 500; i++)
            {
                max = 0;
                for (int j = 0; j < 500; j++)
                {
                    if (matrix[i, j] > max)
                    {
                        max = matrix[i, j];
                    }
                }
                eccentricities.Add(max);
            }

            int diametr = 0;
            int radius = 9999999;
            for (int i = 0; i < 500; i++)
            {
                if (eccentricities[i] > diametr)
                {
                    diametr = eccentricities[i];
                }
                if (radius > eccentricities[i])
                {
                    radius = eccentricities[i];
                }
            }



            for (int i = 0; i < 500; i++)
            {
                if (eccentricities[i] == radius)
                {
                    eccentricitiesAnswer.Add(list[i]);
                }
            }
            Console.WriteLine(radius + " - Радиус сети\n" + diametr + " - Диаметр сети");
            eccentricities.Sort();

            var distance = new List<int>();
            for (int i = 0; i < 500; i++)
            {
                for (int j = i + 1; j < 500; j++)
                {
                    distance.Add(matrix[i, j]);
                }
            }
            distance.Sort();
            Console.WriteLine(distance[(int)((double)500 * (500 - 1) / 2 * 0.9)] + " - 90%");






            var LCC = new Dictionary<int, double>();
            double sum = 0;
            foreach (var temp in graph)
            {

                int k = temp.Value.Count;
                if (k < 2)
                {
                    LCC.Add(temp.Key, 0);
                    continue;
                }

                int e = 0;
                for (int i = 0; i < temp.Value.Count; i++)
                {
                    e += graph[temp.Value[i]].Intersect(temp.Value).ToList().Count;
                }
                sum += (double)2 * e / (k * (k - 1));
                LCC.Add(temp.Key, (double)2 * e / (k * (k - 1)));
            }

            long triangles = numberOfTriangles();


            double arround = 0;
            foreach (var a in LCC)
            {
                arround += a.Value;
            }
            arround /= vertex;

            double global = 0;
            double sumTemp = 0;
            foreach (var i in graph)
            {
                global += graph[i.Key].Count * graph[i.Key].Count * LCC[i.Key];
                sumTemp += graph[i.Key].Count * graph[i.Key].Count;
            }
            global /= sumTemp;

            Console.WriteLine(triangles + " - Число треугольников");
            Console.WriteLine(arround + " - Средний кластерный коэфициент\n " + global + " - Глобальный кластерный коэфициент");

            

        }
    }
}
