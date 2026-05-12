using UnityEngine;
using System.Collections.Generic;

public class MemoryDatabase : MonoBehaviour
{
    public static MemoryDatabase Instance;

    [System.Serializable]
    public class MemoryEntry
    {
        public NPC npc;
        public Sprite memory;
    }

    public List<MemoryEntry> memories = new List<MemoryEntry>();

    void Awake()
    {
        Instance = this;
    }

    public Sprite GetMemory(NPC npc)
    {
        var entry = memories.Find(m => m.npc == npc);
        return entry != null ? entry.memory : null;
    }
}