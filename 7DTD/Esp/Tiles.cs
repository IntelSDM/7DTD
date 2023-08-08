using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    class Tiles : MonoBehaviour
    {

        private float CacheTime;
        public static List<TileEntity> TileList = new List<TileEntity>();
        void Update()
        {
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check if world is active
                if (!(Time.time > CacheTime))
                    return; // check if cache time has passed
                TileList.Clear();
                foreach (ChunkGameObject chunkobj in Globals.LocalPlayer.world.m_ChunkManager.GetDisplayedChunkGameObjects())
                {
                    Chunk chunk = chunkobj.chunk;
                    var tileentities = chunk.GetTileEntities();
                    for (int k = 0; k < tileentities.list.Count; k++)
                    {
                        TileEntity ent = tileentities.list[k];
                        TileList.Add(ent);
                    }
                }

                CacheTime = Time.time + 3; // set the next time we will cache
            }
            catch
            {
            }
        }
        
        void OnGUI()
        {
            try
            {

                if (GameManager.Instance.World == null)
                    return; // check if world is active


                foreach (TileEntity tile in TileList)
                {
                    if (tile == null)
                        continue; // check for valid pointer
                    
                    Vector3i pos = tile.ToWorldPos();
                    Vector3 worldpos = new Vector3(tile.GetChunk().ChunkPos.x + tile.localChunkPos.x, tile.GetChunk().ChunkPos.y, tile.GetChunk().ChunkPos.z + tile.localChunkPos.z);
                    
                
                    Vector3 screenposition = Globals.WorldPointToScreenPoint(worldpos);
                    if (!(Globals.IsScreenPointVisible(screenposition)))
                        continue;
                    int distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, worldpos);
                    string distancestr = Globals.Config.Tiles.Distance ? $"({distance.ToString()}m)" : "";
                    string namestr = Globals.Config.Tiles.Name ? $"{tile.blockValue.Block.GetBlockName()}" : "";
                    if (distance > Globals.Config.Tiles.MaxDistance)
                        continue;

                    Drawing.DrawString(new Vector2(screenposition.x, screenposition.y), $"{namestr}{distancestr}", Helpers.ColourHelper.GetColour("Tile Item Colour"), true, 12, FontStyle.Normal, 0);
                }
            }
            catch { }
        }
       
           
        

    }
}
