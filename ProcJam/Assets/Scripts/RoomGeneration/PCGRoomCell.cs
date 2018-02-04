using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BasicPCG
{
    public enum TileType
    {
        Empty,
        Floor,
        FloorDecoration,
        FloorObstacle,
        Wall,
        WallDecoration,
        Door
    }

    public enum AlignmentType
    {
        Default,
        Horizontal,
        Vertical
    }

    public struct PCGRoomCell
    {
        public TileType tileType;
        public AlignmentType aligment;
        public Tile tile;
        public bool isCorner;
        public Tilemap tilemap;

        public void SetCell(TileType type, AlignmentType aligment, Tilemap tilemap, Tile tile)
        {
            this.tileType = type;
            this.aligment = aligment;
            this.tile = tile;
            this.tilemap = tilemap;
        }

        public void SetCell(TileType type, AlignmentType aligment, Tilemap tilemap, Tile[] tileSet)
        {
            this.tileType = type;
            this.aligment = aligment;
            this.tilemap = tilemap;
            this.tile = tileSet[Random.Range(0, tileSet.Length)];
        }

        public void Draw(int x, int y)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        public void Draw(int x, int y, Tilemap tilemap)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

    }
}
