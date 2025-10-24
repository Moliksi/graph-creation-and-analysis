using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GRAF
{
    public partial class Form1 : Form
    {
        DrawGraph G;
        List<Vertex> V;
        List<Edge> E;
        int[,] AdjacencyMatrix; //матрица смежности
        int[,] IncidenceMatrix; //матрица инцидентности

        int selected1; //выбранные вершины, для соединения линиями
        int selected2;

        public Form1()
        {
            InitializeComponent();
            V = new List<Vertex>();
            G = new DrawGraph(sheet.Width, sheet.Height);
            E = new List<Edge>();
            sheet.Image = G.GetBitmap();
        }

        //кнопка - выбрать вершину
        private void selectButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = false;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            button1.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
        }

        //кнопка - рисовать вершину
        private void drawVertexButton_Click(object sender, EventArgs e)
        {
            drawVertexButton.Enabled = false;
            selectButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            button1.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - рисовать ребро
        private void drawEdgeButton_Click(object sender, EventArgs e)
        {
            drawEdgeButton.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            deleteButton.Enabled = true;
            button1.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
            selected2 = -1;
        }

        //кнопка - удалить элемент
        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteButton.Enabled = false;
            selectButton.Enabled = true;
            button1.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - удалить граф
        private void deleteALLButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            button1.Enabled = true;
            const string message = "Вы действительно хотите полностью удалить граф?";
            const string caption = "Удаление";
            var MBSave = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (MBSave == DialogResult.Yes)
            {
                V.Clear();
                E.Clear();
                G.clearSheet();
                sheet.Image = G.GetBitmap();
            }
        }

        //кнопка - матрица смежности
        private void buttonAdj_Click(object sender, EventArgs e)
        {
            createAdjAndOut();
        }

        //кнопка - матрица инцидентности 
        private void buttonInc_Click(object sender, EventArgs e)
        {
            createIncAndOut();
        }

        private void sheet_MouseClick(object sender, MouseEventArgs e)
        {
            //нажата кнопка "выбрать вершину", ищем степень вершины
            if (selectButton.Enabled == false)
            {
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        if (selected1 != -1)
                        {
                            selected1 = -1;
                            G.clearSheet();
                            G.drawALLGraph(V, E);
                            sheet.Image = G.GetBitmap();
                        }
                        if (selected1 == -1)
                        {
                            G.drawSelectedVertex(V[i].x, V[i].y);
                            selected1 = i;
                            sheet.Image = G.GetBitmap();
                            createAdjAndOut();
                            listBoxMatrix.Items.Clear();
                            int degree = 0;
                            for (int j = 0; j < V.Count; j++)
                                degree += AdjacencyMatrix[selected1, j];
                            listBoxMatrix.Items.Add("Количество ребер вершины №" + (selected1 + 1) + " равно " + degree);
                            break;
                        }
                    }
                }
            }
            //Нажата кнопка "Выбрать две вершины"
            if (button1.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                        {

                            if (selected1 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected1 = i;
                                sheet.Image = G.GetBitmap();
                                createAdjAndOut();
                                break;
                            }
                            if (selected2 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected2 = i;
                                sheet.Image = G.GetBitmap();
                                createAdjAndOut();
                                break;
                            }
                        }
                    }
                }
            }
            //нажата кнопка "рисовать вершину"
            if (drawVertexButton.Enabled == false)
            {
                V.Add(new Vertex(e.X, e.Y));
                G.drawVertex(e.X, e.Y, V.Count.ToString());
                sheet.Image = G.GetBitmap();
            }
            //нажата кнопка "рисовать ребро"
            if (drawEdgeButton.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                        {
                            if (selected1 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected1 = i;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                            if (selected2 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected2 = i;
                                E.Add(new Edge(selected1, selected2,new Pen(Color.Blue)));
                                G.drawEdge(V[selected1], V[selected2], E[E.Count - 1], E.Count - 1);
                                selected1 = -1;
                                selected2 = -1;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    if ((selected1 != -1) &&
                        (Math.Pow((V[selected1].x - e.X), 2) + Math.Pow((V[selected1].y - e.Y), 2) <= G.R * G.R))
                    {
                        G.drawVertex(V[selected1].x, V[selected1].y, (selected1 + 1).ToString());
                        selected1 = -1;
                        sheet.Image = G.GetBitmap();
                    }
                }
            }
            //нажата кнопка "удалить элемент"
            if (deleteButton.Enabled == false)
            {
                bool flag = false; //удалили ли что-нибудь по ЭТОМУ клику
                //ищем, возможно была нажата вершина
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        for (int j = 0; j < E.Count; j++)
                        {
                            if ((E[j].v1 == i) || (E[j].v2 == i))
                            {
                                E.RemoveAt(j);
                                j--;
                            }
                            else
                            {
                                if (E[j].v1 > i) E[j].v1--;
                                if (E[j].v2 > i) E[j].v2--;
                            }
                        }
                        V.RemoveAt(i);
                        flag = true;
                        break;
                    }
                }
                //ищем, возможно было нажато ребро
                if (!flag)
                {
                    for (int i = 0; i < E.Count; i++)
                    {
                        if (E[i].v1 == E[i].v2) //если это петля
                        {
                            if ((Math.Pow((V[E[i].v1].x - G.R - e.X), 2) + Math.Pow((V[E[i].v1].y - G.R - e.Y), 2) <= ((G.R + 2) * (G.R + 2))) &&
                                (Math.Pow((V[E[i].v1].x - G.R - e.X), 2) + Math.Pow((V[E[i].v1].y - G.R - e.Y), 2) >= ((G.R - 2) * (G.R - 2))))
                            {
                                E.RemoveAt(i);
                                flag = true;
                                break;
                            }
                        }
                        else //не петля
                        {
                            if (((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) <= (e.Y + 4) &&
                                ((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) >= (e.Y - 4))
                            {
                                if ((V[E[i].v1].x <= V[E[i].v2].x && V[E[i].v1].x <= e.X && e.X <= V[E[i].v2].x) ||
                                    (V[E[i].v1].x >= V[E[i].v2].x && V[E[i].v1].x >= e.X && e.X >= V[E[i].v2].x))
                                {
                                    E.RemoveAt(i);
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                //если что-то было удалено, то обновляем граф на экране
                if (flag)
                {
                    G.clearSheet();
                    G.drawALLGraph(V, E);
                    sheet.Image = G.GetBitmap();
                }
            }
        }

        //создание матрицы смежности и вывод в листбокс
        private void createAdjAndOut()
        {
            AdjacencyMatrix = new int[V.Count, V.Count];
            G.fillAdjacencyMatrix(V.Count, E, AdjacencyMatrix);
            listBoxMatrix.Items.Clear();
            string sOut = "    ";
            for (int i = 0; i < V.Count; i++)
                sOut += (i + 1) + " ";
            listBoxMatrix.Items.Add(sOut);
            for (int i = 0; i < V.Count; i++)
            {
                sOut = (i + 1) + " | ";
                for (int j = 0; j < V.Count; j++)
                    sOut += AdjacencyMatrix[i, j] + " ";
                listBoxMatrix.Items.Add(sOut);
            }
        }

        //создание матрицы инцидентности и вывод в листбокс
        private void createIncAndOut()
        {
            if (E.Count > 0)
            {
                IncidenceMatrix = new int[V.Count, E.Count];
                G.fillIncidenceMatrix(V.Count, E, IncidenceMatrix);
                listBoxMatrix.Items.Clear();
                string sOut = "    ";
                for (int i = 0; i < E.Count; i++)
                    sOut += (char)('a' + i) + " ";
                listBoxMatrix.Items.Add(sOut);
                for (int i = 0; i < V.Count; i++)
                {
                    sOut = (i + 1) + " | ";
                    for (int j = 0; j < E.Count; j++)
                        sOut += IncidenceMatrix[i, j] + " ";
                    listBoxMatrix.Items.Add(sOut);
                }
            }
            else
                listBoxMatrix.Items.Clear();
        }


        private void chainButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                createAdjAndOut();
                DijkstraAlgorithm(selected1);
            }
            catch
            {
                MessageBox.Show("Не выбрана вершина");
            }
           
        }
        // Алгоритм Дейкстры для поиска кратчайшего пути в графе от заданной вершины до всех других вершин
        private void DijkstraAlgorithm(int startVertex)
        {
            // Инициализация массива для хранения кратчайших расстояний от стартовой вершины до каждой вершины
            int[] distances = new int[V.Count];
            for (int i = 0; i < V.Count; i++)
            {
                distances[i] = int.MaxValue; // Изначально расстояния считаются бесконечными
            }
            distances[startVertex] = 0; // Расстояние от стартовой вершины до самой себя равно 0

            // Инициализация массива для хранения посещенных вершин
            bool[] visited = new bool[V.Count];

            // Цикл, выполняющийся V.Count раз, чтобы посетить все вершины
            for (int count = 0; count < V.Count - 1; count++)
            {
                // Находим вершину с минимальным расстоянием, которая еще не была посещена
                int minDistance = int.MaxValue;
                int minDistanceVertex = -1;
                for (int v = 0; v < V.Count; v++)
                {
                    if (!visited[v] && distances[v] <= minDistance)
                    {
                        minDistance = distances[v];
                        minDistanceVertex = v;
                    }
                }

                // Посетили найденную вершину
                visited[minDistanceVertex] = true;

                // Обновляем расстояния до смежных вершин, если новый путь короче
                for (int v = 0; v < V.Count; v++)
                {
                    if (!visited[v] && AdjacencyMatrix[minDistanceVertex, v] != 0 && distances[minDistanceVertex] != int.MaxValue
                        && distances[minDistanceVertex] + AdjacencyMatrix[minDistanceVertex, v] < distances[v])
                    {
                        distances[v] = distances[minDistanceVertex] + AdjacencyMatrix[minDistanceVertex, v];
                    }
                }
            }
            // Выводим кратчайшие расстояния до всех вершин
            listBoxMatrix.Items.Clear();
            for (int i = 0; i < V.Count; i++)
            {
                listBoxMatrix.Items.Add("Кратчайшее расстояние от вершины " + (startVertex + 1) + " до вершины " + (i + 1) + ": " + distances[i]);
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            deleteButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
            selected2 = -1;
        
        }
        
        private void DijkstraAlgorithm(int startVertex, int endVertex)
        {
            // Инициализация массива для хранения кратчайших расстояний от стартовой вершины до каждой вершины
            int[] distances = new int[V.Count];
            string[] path = new string[V.Count]; // Массив для хранения кратчайшего пути до каждой вершины
            bool[] visited = new bool[V.Count];
            string startVertex1 = (startVertex + 1).ToString();
            // Инициализация расстояний и пути
            for (int i = 0; i < V.Count; i++)
            {
                distances[i] = int.MaxValue;
                path[i] = startVertex1; // Изначально пути неизвестны
            }

            distances[startVertex] = 0; // Расстояние от стартовой вершины до самой себя равно 0

            // Цикл, выполняющийся V.Count раз, чтобы посетить все вершины
            for (int count = 0; count < V.Count - 1; count++)
            {
                // Находим вершину с минимальным расстоянием, которая еще не была посещена
                int minDistance = int.MaxValue;
                int minDistanceVertex = -1;
                for (int v = 0; v < V.Count; v++)
                {
                    if (!visited[v] && distances[v] <= minDistance)
                    {
                        minDistance = distances[v];
                        minDistanceVertex = v;
                    }
                }

                // Посетили найденную вершину
                visited[minDistanceVertex] = true;
                // Обновляем расстояния до смежных вершин, если новый путь короче
                for (int v = 0; v < V.Count; v++)
                {
                    if (!visited[v] && AdjacencyMatrix[minDistanceVertex, v] != 0 && distances[minDistanceVertex] != int.MaxValue
                        && distances[minDistanceVertex] + AdjacencyMatrix[minDistanceVertex, v] < distances[v])
                    {
                        distances[v] = distances[minDistanceVertex] + AdjacencyMatrix[minDistanceVertex, v];
                        path[v] = path[minDistanceVertex] + ">" + (v + 1).ToString(); // Обновляем кратчайший путь
                    }
                }
                
            }

            // Выводим кратчайший путь и расстояние до конечной вершины
            listBoxMatrix.Items.Clear();
            listBoxMatrix.Items.Add("Кратчайший путь от вершины " + (startVertex + 1) + " до вершины " + (endVertex + 1) + ": " + path[endVertex]);
            listBoxMatrix.Items.Add("Кратчайшее расстояние: " + distances[endVertex]);
            string[] paths = path[endVertex].Split('>');
            int[] path1 = new int[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                int.TryParse(paths[i], out path1[i]);
                path1[i]--;
            }
            for (int i = 0; i < E.Count(); i++)
            {
                // Проверяем, принадлежит ли текущее ребро пути
                if ((E[i].v1 == startVertex && path1.Contains(E[i].v2)) || (path1.Contains(E[i].v1) && path1.Contains(E[i].v2)))
                {
                    E[i].color = new Pen(Color.Red); // Помечаем ребро пути
                }
            }
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }
        public void remove_the_selection ()
        {
            for (int i = 0; i < E.Count(); i++)
            {
                E[i].color = new Pen(Color.Blue);
            }
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();

        }
        private void Button2_Click(object sender, EventArgs e)
        {            
            DijkstraAlgorithm(selected1, selected2);
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            remove_the_selection();
        }
    }
}
