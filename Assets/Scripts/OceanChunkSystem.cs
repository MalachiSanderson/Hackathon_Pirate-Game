using UnityEngine;

public class OceanChunkSystem : MonoBehaviour
{
    public Transform player;
    public GameObject oceanPrefab;
    public Vector3 size = new Vector3(15, 1, 15);
    private Transform[] chunks = new Transform[9];
    private Transform currentChunk;

    private void Awake()
    {
        for (var z = -1; z <= 1; z++)
        {
            for (var x = -1; x <= 1; x++)
            {
                var index = ToIndex(x, z);
                chunks[index] = Instantiate(oceanPrefab, Vector3.zero, Quaternion.identity, transform).transform;
                var pos = Vector3.Scale(size, new Vector3(x, 1, z));
                chunks[index].position = pos;
                chunks[index].name = "Ocean Chunk [" + x + ", " + z + "]";
            }
        }

        if (player == null)
        {
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                player = playerController.transform;

            if (player == null)
                Debug.LogError("Tsk tsk... No player transform assigned to OceanChunkSystem. -Mohammad", gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating("UpdateChunks", 1f, 1f);
    }

    private void UpdateChunks()
    {
        currentChunk = GetCurrentChunk(out var chunkIndex);
        if (currentChunk != null)
            CenterizeChunk(currentChunk, chunkIndex);
    }

    private void CenterizeChunk(Transform playerChunk, int chunkIndex)
    {
        FromIndex(chunkIndex, out var cx, out var cz);

        var newPos = playerChunk.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

        var newChunks = new Transform[9];
        for (var z = -1; z <= 1; z++)
        {
            for (var x = -1; x <= 1; x++)
            {
                var index = ToIndex(x, z);
                var newX = x - cx;
                var newZ = z - cz;
                if (newX < -1) newX += 3;
                if (newZ < -1) newZ += 3;
                if (newX > 1) newX -= 3;
                if (newZ > 1) newZ -= 3;
                var centerizedIndex = ToIndex(newX, newZ);
                newChunks[index] = chunks[centerizedIndex];
            }
        }

        chunks = newChunks;
    }

    private Transform GetCurrentChunk(out int chunkIndex)
    {
        var playerPos = player.position;
        playerPos.y = 0;

        for (var z = -1; z <= 1; z++)
        {
            for (var x = -1; x <= 1; x++)
            {
                var index = ToIndex(x, z);
                var chunk = chunks[index];
                var min = chunk.position - size / 2;
                var max = chunk.position + size / 2;
                min.y = 0;
                max.y = 0;

                if (playerPos.x >= min.x && playerPos.x <= max.x && playerPos.z >= min.z && playerPos.z <= max.z)
                {
                    chunkIndex = index;
                    return chunk;
                }
            }
        }

        chunkIndex = -1;
        return null;
    }

    private static void FromIndex(int index, out int x, out int y)
    {
        /*
         * 0 1 2
         * 3 4 5
         * 6 7 8
         */

        /*
         * -1,1    0,1      1,1
         * -1,0    0,0      1,0
         * -1,-1   0,-1     1,-1
         */

        x = (index % 3) - 1;
        y = 1 - (index / 3);
    }

    private static int ToIndex(int x, int y)
    {
        return (x + 1) + 3 * (1 - y);
    }

    public Collider GetWaterCollider()
    {
        return currentChunk == null ? null : currentChunk.GetChild(0).GetComponent<Collider>();
    }
}
