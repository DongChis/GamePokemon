using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ManagerGame : ChiMonoBehaviour
{

    private static ManagerGame instance;
    public static ManagerGame Instance => instance;
    public int rows = 9;
    public int cols = 16;
    [SerializeField] List<int> values;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Transform blockPrefab;
    [SerializeField] protected List<BlockItem> blockItems;
    [SerializeField] public Sprite[] sprites;

    [SerializeField] public Transform blocks;

    [SerializeField] private List<WeakReference<BlockItem>> clickBlock = new List<WeakReference<BlockItem>>();

    protected override void Awake()
    {
        base.Awake();
        if (ManagerGame.instance != null) Debug.LogError("Only 1 ManagerBlock allow to exist");
        ManagerGame.instance = this;
        sprites = Resources.LoadAll<Sprite>("Images");
        InitializeBlock();


    }

    

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBlockPrefab();
        this.LoadCanvas();
        this.LoadBlocks();
        
    }

    protected override void FixedUpdate()
    {
        check();

    }

    public void AddBlock(BlockItem block)
    {
        if (block == null) return;

        // Sử dụng WeakReference để thêm block
        clickBlock.Add(new WeakReference<BlockItem>(block));
        Debug.Log("Game");

        if (clickBlock.Count == 2)
        {
            BlockItem block1 = null;
            BlockItem block2 = null;

            // Lấy các đối tượng từ WeakReference
            clickBlock[0].TryGetTarget(out block1);
            clickBlock[1].TryGetTarget(out block2);

            // Kiểm tra null và tiếp tục xử lý nếu cả hai block đều tồn tại
            if (block1 != null && block2 != null)
            {
                Debug.Log("Block1id: " + block1.Value);
                Debug.Log("Block2id: " + block2.Value);

                if (block1.Value == block2.Value && CanConnect(block1, block2))
                {
                    block1.gameObject.SetActive(false);
                    block2.gameObject.SetActive(false);
                }
            }

            // Làm trống danh sách clickBlock
            clickBlock.Clear();
        }
    }


    public void check()
    {
        
    }

    public bool CanConnect(BlockItem bock1, BlockItem block2)
    {
        // Sử dụng BFS để tìm đường đi giữa hai ô
        return BreadthFirstSearch(bock1, block2);
    }


    protected void LoadBlocks()
    {
        if (this.blocks != null) return;
        this.blocks = this.canvas.transform.Find("Blocks");
        Debug.LogWarning(transform.name + " LoadBlocks", gameObject);
    }

    protected virtual void LoadCanvas()
    {
        if (this.canvas != null) return;
        this.canvas = ManagerGame.FindObjectOfType<Canvas>();
        Debug.LogWarning(transform.name + " LoadCanvas", gameObject);
    }


    protected void LoadBlockPrefab()
    {
        if (this.blockPrefab != null) return;
        this.blockPrefab = transform.Find("Block");
        Debug.LogWarning(transform.name + " LoadBlockPrefab", gameObject);
    }

    protected override void Start()
    {
        base.Start();

    }

    public void InitializeBlock()
    {


        values = new List<int>();
        int blockCount = 4;
        
        for (int i = 0; i < (rows*cols)/ blockCount ; i++)
        {
            for(int j = 0; j < blockCount; j++)
            {
                values.Add(i);
            }
        }
        ShuffleList(values);    
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Transform card = Instantiate(blockPrefab, new Vector3(j, i, 0), Quaternion.identity);
                int value = values[i * cols + j];
                BlockItem blockItem = card.GetComponent<BlockItem>();
                blockItem.SetValue(value);
                blockItem.setPosition(new Position(i, j));
                blockItems.Add(blockItem);
                card.gameObject.SetActive(true);
                card.SetParent(this.blocks);
                blockItem.GetComponent<Image>().sprite = sprites[value];
            }
        }
       
       
        return;
    }


    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count - 1;
        while (n > 0)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }


    }

    bool BreadthFirstSearch(BlockItem start, BlockItem target)
    {
        Queue<BlockItem> queue = new Queue<BlockItem>();
        HashSet<BlockItem> visited = new HashSet<BlockItem>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            BlockItem current = queue.Dequeue();

            if (current == target)
            {
                return true;
            }

            // Kiểm tra các ô lân cận
            foreach (BlockItem neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        return false;
    }

    List<BlockItem> GetNeighbors(BlockItem block)
    {
        List<BlockItem> neighbors = new List<BlockItem>();

        Position[] directions = { new Position(0,1), new Position(0,-1), new Position(-1,0), new Position(1,0) };

        foreach (Position dir in directions)
        {
            Position neighborPosition = new Position(block.Position.posX + dir.posX, block.Position.posY + dir.posY);

            // Kiểm tra xem ô lân cận có nằm trong phạm vi lưới không
            if (IsWithinGrid(neighborPosition))
            {
                BlockItem neighbor = GetBlockItemAtPosition(neighborPosition);
                if (neighbor != null) neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    bool IsWithinGrid(Position position)
    {
            return position.posX >= 0 && position.posX < rows && position.posY >= 0 && position.posY < cols;
    }

    BlockItem GetBlockItemAtPosition(Position position)
    {
        // Duyệt qua danh sách các BlockItem và tìm phần tử có vị trí trùng khớp
        foreach (BlockItem block in blockItems)
        {
            if (block.position.posX == position.posX && block.position.posY == position.posY)
            {
                return block;
            }
        }

        return null;
    }   
}
