using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DeskretnaLab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("(\\d+)-(\\d+) (\\d);");
            Match match = regex.Match(richTextBox1.Text);
            int[,] nArr = new int[regex.Matches(richTextBox1.Text).Count, 3];
            richTextBox2.Text += "Входные данные: \n";
            for (int i = 0; match.Success && i < nArr.GetLength(0); i++)
            {
                for (int j = 0; j < nArr.GetLength(1); j++)
                    nArr[i, j] = int.Parse(match.Groups[j + 1].Value);
                richTextBox2.Text += $"Ребро: {nArr[i, 0]}-{nArr[i, 1]}, вес: {nArr[i, 2]}\n";
                match = match.NextMatch();
            }
            int[] tmpVertexArr = new int[regex.Matches(richTextBox1.Text).Count * 2];
            bool isRepeat;
            int vertexAmount = 0;
            for (int i = 0; i < nArr.GetLength(0); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    isRepeat = false;
                    foreach (int x in tmpVertexArr) 
                        if (x == nArr[i, j])
                        {
                            isRepeat = true;
                            break;
                        }
                    if (!isRepeat)
                    {
                        tmpVertexArr[vertexAmount] = nArr[i, j];
                        vertexAmount++;
                    }
                }
            }
            int[] vertexArr = new int[vertexAmount];
            int counter = 0;
            foreach (int x in tmpVertexArr)
            {
                if (x != 0) vertexArr[counter] = x;
                counter++;
            }
            Array.Sort(vertexArr);
            richTextBox2.Text += "\nИтак, имеем вершины:\n";
            foreach (int l in vertexArr) richTextBox2.Text += " " + l.ToString();
            //массив хранит таблицу смежности, значения массива - вес ребра, 0 - если ребро отсутствует
            int[,] adjacencyTable = new int[vertexArr.Length+1, vertexArr.Length+1];
            //цикл вводит вершины в таблицу
            for (int x = 1; x < adjacencyTable.GetLength(0); x++)
            {
                adjacencyTable[0, x] = vertexArr[x - 1];
                adjacencyTable[x, 0] = vertexArr[x - 1];
            }
            for (int i = 0; i < nArr.GetLength(0); i++)
            {
                //цикл по горизонтали ищет название первой вершины из nArr
                for (int x = 1; x < adjacencyTable.GetLength(0); x++)
                {
                    if (nArr[i, 0] == adjacencyTable[0, x])
                    {
                        // если находит, то цикл ищет название второй вершины из nArr
                        for (int y = 1; y < adjacencyTable.GetLength(1); y++)
                        {
                            if (nArr[i, 1] == adjacencyTable[0, y])
                            {
                                //если находит, то вписывает вес ребра между вершинами
                                adjacencyTable[x, y] = nArr[i, 2];
                                //из-за того, что граф неориентирован, то он симетричен
                                adjacencyTable[y, x] = nArr[i, 2];
                            }
                        }
                    }
                }
            }
            richTextBox2.Text += "\n\nТаблица смежности с весом ребер: \n";
            for (int i = 0; i < adjacencyTable.GetLength(0); i++)
            {
                if (i < 10) richTextBox2.Text += " ";
                for (int j = 0; j < adjacencyTable.GetLength(1); j++)
                {
                    richTextBox2.Text += "  " + adjacencyTable[i, j];
                }
                richTextBox2.Text += "\n";
            }
            int[,] tmpAdjacencyTable = adjacencyTable;
            int vertexLeft = vertexArr.Length;
            int[] connectedVertex = new int[vertexArr.Length];
            int connectedVertexCounter = 0;
            richTextBox2.Text += "\nДействуем за алгоритмом Прима, начнём с вершины " + vertexArr[0];
            int sum = 0;
            connectedVertex[connectedVertexCounter] = vertexArr[0];
            connectedVertexCounter++;
            while (connectedVertexCounter != vertexArr.Length)
            {
                int minWeight = 999999;
                int minWeightIndexI = -1;
                int minWeightIndexJ = -1;
                for (int i = 0; i < connectedVertexCounter; i++)
                {
                    for (int j = 1; j <= vertexArr.Length; j++)
                    {
                        int x = tmpAdjacencyTable[connectedVertex[i], j];
                        if (x < minWeight && x != 0)
                        {
                            bool isAlreadyOn = false;
                            foreach (int l in connectedVertex) if (l == tmpAdjacencyTable[0, j]) isAlreadyOn = true;
                            if (!isAlreadyOn)
                            {
                                minWeight = x;
                                minWeightIndexI = connectedVertex[i];
                                minWeightIndexJ = j;
                            }
                            
                        }
                    }
                }
                connectedVertex[connectedVertexCounter] = tmpAdjacencyTable[minWeightIndexJ, 0];
                connectedVertexCounter++;
                sum += tmpAdjacencyTable[minWeightIndexI, minWeightIndexJ];
                richTextBox2.Text += $"\nРебро с минимальным весом: {tmpAdjacencyTable[minWeightIndexI, 0]}-" +
                    $"{tmpAdjacencyTable[0, minWeightIndexJ]}, вес: {tmpAdjacencyTable[minWeightIndexI, minWeightIndexJ]}\n" +
                    $"cумма: {sum}";
                tmpAdjacencyTable[minWeightIndexI, minWeightIndexJ] = 0;
                tmpAdjacencyTable[minWeightIndexJ, minWeightIndexI] = 0;
            }
        }
    }  
}


