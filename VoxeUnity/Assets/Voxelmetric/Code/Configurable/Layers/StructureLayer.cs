﻿using System;
using UnityEngine;
using Voxelmetric.Code;
using Voxelmetric.Code.Common;
using Voxelmetric.Code.Common.Math;
using Voxelmetric.Code.Core;
using Voxelmetric.Code.Data_types;
using Voxelmetric.Code.Load_Resources;

public class StructureLayer : TerrainLayer
{
    protected GeneratedStructure structure;
    private float chance;

    protected override void SetUp(LayerConfig config)
    {
        // Config files for random layers MUST define these properties
        chance = float.Parse(properties["chance"]);

        var structureType = Type.GetType(config.structure + ", " + typeof(GeneratedStructure).Assembly, false);
        if (structureType==null)
        {
            Debug.LogError("Could not create structure "+config.structure);
            return;
        }

        structure = (GeneratedStructure)Activator.CreateInstance(structureType);
    }

    public override void Init(LayerConfig config)
    {
        structure.Init(world);
    }

    public override float GetHeight(Chunk chunk, int layerIndex, int x, int z, float heightSoFar, float strength)
    {
        return heightSoFar;
    }

    public override float GenerateLayer(Chunk chunk, int layerIndex, int x, int z, float heightSoFar, float strength)
    {
        return heightSoFar;
    }

    public override void GenerateStructures(Chunk chunk, int layerIndex)
    {
        //if (chunk.pos.x!=-30 || chunk.pos.y!=30 || chunk.pos.z!=0) return;

        int minX = chunk.pos.x;
        int maxX = chunk.pos.x + Env.ChunkSize1;
        int minZ = chunk.pos.z;
        int maxZ = chunk.pos.z + Env.ChunkSize1;

        int structureID = 0;

        for (int x = minX; x<=maxX; x++)
        {
            for (int z = minZ; z<=maxZ; z++)
            {
                Vector3Int pos = new Vector3Int(x, 0, z);
                float chanceAtPos = Randomization.RandomPrecise(pos.GetHashCode(), 44);

                if (chance>chanceAtPos)
                {
                    if (Randomization.RandomPrecise(pos.Add(1, 0, 0).GetHashCode(), 44)>chanceAtPos &&
                        Randomization.RandomPrecise(pos.Add(-1, 0, 0).GetHashCode(), 44)>chanceAtPos &&
                        Randomization.RandomPrecise(pos.Add(0, 0, 1).GetHashCode(), 44)>chanceAtPos &&
                        Randomization.RandomPrecise(pos.Add(0, 0, -1).GetHashCode(), 44)>chanceAtPos)
                    {
                        int xx = Helpers.Mod(x, Env.ChunkSize);
                        int zz = Helpers.Mod(z, Env.ChunkSize);
                        int height = Helpers.FastFloor(terrainGen.GetTerrainHeightForChunk(chunk, xx, zz));
                        
                        Vector3Int worldPos = new Vector3Int(x, height, z);
                        structure.Build(chunk, structureID++, ref worldPos, this);
                    }
                }
            }
        }
    }
}