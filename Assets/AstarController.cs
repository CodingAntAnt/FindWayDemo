using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarController : MonoBehaviour
{

    void Start()
    {
        Astar a = new Astar();
        a.FindNode(3, 1, 4, 4);
        a.PrintMap();
    }
}

class Astar
{
    List<Node> OpenList = new List<Node>();
    List<Node> CloseList = new List<Node>();
    const byte MaxX = 10;
    const byte MaxY = 10;

    byte[,] Map = new byte[MaxX, MaxY] {
            { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }

            };

    public void FindNode(int x1, int y1, int x2, int y2)
    {
        Node start = new Node(x1, y1);
        Node end = new Node(x2, y2);

        Node Templet = CheckNode(start, end);

        while (Templet != null)
        {
            Map[Templet.x, Templet.y] = 3;
            Templet = Templet.father;
        }

    }

    //从开启列表返回对应坐标的点
    private Node GetPointFromOpenList(int x, int y)
    {
        CloseList.Find((n) =>
        {
            return n.x == x && n.y == y;
        });

        return null;
    }

    //获取一个点四周的点
    List<Node> GetNodesAround(Node node)
    {
        List<Node> list = new List<Node>();
        for (int i = node.x - 1; i <= node.x + 1; i++)
        {
            //过滤超出边界的点
            if (i < 0 || i > MaxX - 1) continue;

            for (int j = node.y - 1; j <= node.y + 1; j++)
            {
                //过滤超出边界的点
                if (j < 0 || j > MaxY - 1) continue;
                //过滤障碍
                if (Map[i, j] == 0) continue;
                //过滤CloseList
                if (IsInCloseList(i, j)) continue;
                //过滤自己
                if (i == node.x && j == node.y) continue;
                list.Add(new Node(i, j, node));
            }
        }
        return list;
    }

    Node CheckNode(Node start, Node end)
    {
        OpenList.Add(start);
        while (OpenList.Count != 0)
        {
            Node minNode = GetMinFFromOpenList();
            OpenList.Remove(minNode);
            CloseList.Add(minNode);
            var list = GetNodesAround(minNode);

            foreach (var node in list)
            {
                //计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                if (OpenList.Contains(node))
                {
                    int G = CaluG(node, minNode);
                    if (G < node.G)
                    {
                        node.G = G;
                        node.father = minNode;
                    }
                }
                //如果它们不在开始列表里, 就加入, 并设置父节点,并计算GHF
                else
                {
                    node.G = CaluG(node);
                    node.H = CaluH(node, end);
                    OpenList.Add(node);
                }
            }

            //如果end点在OpenList中，寻路结束
            Node finish = GetNodeInOpenList(end.x, end.y);
            if (finish != null)
                return finish;
        }

        return GetNodeInOpenList(end.x, end.y);
    }

    int CaluG(Node node, Node father = null)
    {
        if (father != null)
        {
            if (node.x != father.x && node.y != father.y)
                return father.G + 14;

            return father.G + 10;
        }

        if (node.father == null) return 0;

        if (node.x != node.father.x && node.y != node.father.y)
            return node.father.G + 14;

        return node.father.G + 10;
    }

    int CaluH(Node from, Node to)
    {
        return 10 * (Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y));
    }

    private Node GetNodeInOpenList(int x, int y)
    {
        Node node = OpenList.Find((Node n) => { return n.x == x && n.y == y; });
        return node;
    }

    // 获取F值最小的点
    private Node GetMinFFromOpenList()
    {
        OpenList.Sort((n1, n2) => { return n1.F - n2.F; });
        return OpenList[0];
    }

    private bool IsInCloseList(int x, int y)
    {
        Node node = CloseList.Find((Node n) => { return n.x == x && n.y == y; });
        return node != null;
    }

    public void PrintMap()
    {
        string ret = "";
        for (int a = 0; a < 10; a++)
        {
            for (int b = 0; b < 10; b++)
            {
                if (Map[a, b] == 1) ret += "█";
                else if (Map[a, b] == 3) ret += "<color=#9400D3>" + "█" + "</color>";
                else ret += "  ";
            }
            ret += "\n";
        }

        Debug.Log(ret);
    }

    class Node
    {
        public Node father;
        public int x;
        public int y;
        public int G;
        public int H;
        public int F { get { return G + H; } }

        public Node(int x, int y, Node P = null)
        {
            this.x = x;
            this.y = y;
            father = P;
        }
    }
}
