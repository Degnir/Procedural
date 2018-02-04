using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BasicPCG
{
    public class PCGRoom : MonoBehaviour
    {
        [Range(2, 15)]
        public int maxRoomWidth;
        [Range(2, 15)]
        public int maxRoomHeight;

        [Range(1, 4)]
        public int numberOfDoors;

        public Tile backgroundTile;

        public Tilemap floorTilemap;
        public Tilemap wallsTilemap;
        public Tile[] roomFloor;
        public Tile[] roomHorizontalWalls;
        public Tile[] roomVerticalWalls;
        public Tile doorVertical;
        public Tile doorHorizontal;

        private PCGRoomCell[,] room;

        private int roomWidth;
        private int roomHeight;

        void Start()
        {
            PrepareToGenerate();
        }

        public void PrepareToGenerate()
        {
            floorTilemap.ClearAllTiles();
            wallsTilemap.ClearAllTiles();

            roomWidth = Random.Range(2, maxRoomWidth) + 1;
            roomHeight = Random.Range(2, maxRoomHeight) + 1;
            room = new PCGRoomCell[roomWidth, roomHeight];

            GenerateRandomFloorTiles(); 
        }

        private void GenerateRandomFloorTiles()
        {
            for (int width = 0; width < roomWidth - 1; ++width)
            {
                for (int height = 0; height < roomHeight - 1; ++height)
                {
                    room[width, height].SetCell(TileType.Floor, AlignmentType.Default, floorTilemap, roomFloor);
                }
            }
            GenerateRandomWallTiles();
            //Draw();
        }

        private void GenerateRandomWallTiles()
        {
            for (int width = 0; width < roomWidth; ++width)
            {
                for (int height = 0; height < roomHeight; ++height)
                {
                    if(width == 0 && (height == 0 || height == roomHeight - 1) 
                        || width == roomWidth - 1 && (height == 0 || height == roomHeight - 1))
                    {
                        room[width, height].isCorner = true;
                    }

                    if (width == 0 || width == roomWidth - 1)
                    {
                        room[width, height].SetCell(TileType.Wall, AlignmentType.Vertical, wallsTilemap, roomVerticalWalls);
                    }

                    if (height == 0 || height == roomHeight - 1 && (width != roomWidth - 1 && width != 0))
                    {
                        room[width, height].SetCell(TileType.Wall, AlignmentType.Horizontal, wallsTilemap, roomHorizontalWalls);
                    }
                }
            }
            //Draw();
            GenerateDoorTiles();
        }

        private void GenerateDoorTiles()
        {
            int doorCount = 0;
            bool restertLoop = false;

            while (doorCount < numberOfDoors)
            {
                for (int width = 0; width < roomWidth; ++width)
                {
                    for (int height = 0; height < roomHeight; ++height)
                    {
                        if (room[width, height].tileType == TileType.Wall && !room[width, height].isCorner)
                        {
                            if(Random.Range(0,100) > 70)
                            {
                                doorCount++;
                                restertLoop = true;
                                if (room[width, height].aligment == AlignmentType.Horizontal)
                                {
                                    room[width, height].SetCell(TileType.Door, AlignmentType.Horizontal, floorTilemap, roomFloor);
                                }
                                else
                                {
                                    if (room[width, height + 1].tileType == TileType.Wall)
                                    {
                                        room[width, height + 1].SetCell(TileType.Wall, AlignmentType.Horizontal, wallsTilemap, roomHorizontalWalls);
                                    }

                                    room[width, height].SetCell(TileType.Door, AlignmentType.Vertical, floorTilemap, roomFloor);
                                }
                                break;
                            }
                        }
                    }
                    if(restertLoop)
                    {
                        restertLoop = false;
                        break;
                    }
                }
            }
            Draw();
        }

        private void Draw()
        {
            for (int width = 0; width < roomWidth; ++width)
            {
                for (int height = 0; height < roomHeight; ++height)
                {
                    room[width, height].Draw(width, height);
                }
            }

        }
    }
}