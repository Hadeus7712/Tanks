using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects;
using Server.Objects.Types;

namespace Server
{
    class SceneProcessing
    {
        private Server _server;
        private SceneGenerator _sceneGenerator;

        public SceneProcessing()
        {
            
            _server = new Server("127.0.0.1", 5555);
            Console.WriteLine("server created");
            _server.InitializeClient();
            _server.SendToClients();
            Console.WriteLine("new client");
            _sceneGenerator = new SceneGenerator();
            _server.InitializeObjects(_sceneGenerator.GetMapPoints());
            _server.InitializeSettings(_sceneGenerator.Settings);
            Console.WriteLine("map loaded");
            _sceneGenerator.GenerateField();
            _sceneGenerator.GeneratePlayers(_server.Players);
        }

        public void RunProccessing()
        {
            while (true)
            {
                Update();
            }
        }
        private void Update()
        {
            _server.ReceiveFromClients();
            //Console.WriteLine(_server.ServerState);
            UpdatePlayers();
            Intersections();
            _server.SendToClients();
            _server.UpdateMap(_sceneGenerator.CurrentIndexesForDelete.ToArray());

            _sceneGenerator.IndexerReset();
            //_sceneGenerator.UpdateDeleterIndexes(-1);
        }

        public void UpdatePlayers()
        {
            for (int i = 0; i < _server.Players.Count; i++)
            {
                UpdatePlayersMove(_server.Players[i]);
                UpdatePlayersProjectile(_server.Players[i]); 
                _server.Players[i].UpdatePlayerStats();
            }
        }
        public void UpdatePlayersMove(Player player)
        {
            if (player.dataInput.CurrentInput != 0)
            {
                if (player.dataInput.CurrentInput == (int)InputKeyValues.W
                    && player.GetDirectionBlocker((int)Directions.Up))
                {
                    player.ChangeDirection(player.dataInput.CurrentDirection);
                    player.ChangePosition(0, -1);
                }
                else if (player.dataInput.CurrentInput == (int)InputKeyValues.A
                    && player.GetDirectionBlocker((int)Directions.Left))
                {
                    player.ChangeDirection(player.dataInput.CurrentDirection);
                    player.ChangePosition(-1, 0);
                }
                else if (player.dataInput.CurrentInput == (int)InputKeyValues.S
                    && player.GetDirectionBlocker((int)Directions.Down))
                {
                    player.ChangeDirection(player.dataInput.CurrentDirection);
                    player.ChangePosition(0, 1);
                }
                else if (player.dataInput.CurrentInput == (int)InputKeyValues.D
                    && player.GetDirectionBlocker((int)Directions.Right))
                {
                    player.ChangeDirection(player.dataInput.CurrentDirection);
                    player.ChangePosition(1, 0);
                }
                else
                {
                    player.ChangeDirection(player.dataInput.CurrentDirection);
                    player.ChangePosition(0, 0);
                }
            }
        }

        public void UpdatePlayersProjectile(Player player)
        {
            if (player.dataInput.IsShooting != 0)
            {
                //player.DisplayToggle(true);
                //player.ProjectileGenLockerToggle(true);
                player.ChangeShootingToggle(true);
                //_sceneGenerator.GenerateProjectile(player);
                player.SetProjectilePosition();
            }
            else
            {
                player.ChangeShootingToggle(false);
            }
            player.ChangeProjectilePosition();
        }

        private void Intersections()
        {
            for (int i = 0; i < _server.Players.Count; i++)
            {
                OnlockDirectionBlocker(_server.Players[i]);
                LockDirectionBlocker(_server.Players[i]);
                UpdateBlockProjectilesIntersection(_server.Players[i]);
            }
        }
        private void LockDirectionBlocker(Player player)
        {
            List<Tuple<Directions, Block>> tupleDB = Collision2D.CollisionDirectionsArrayDlS(player.GameObject, _sceneGenerator.GameObjects);
            if (tupleDB.Count > 0 && !tupleDB[0].Item2.Equals(player.GameObject))
            {
                for (int i = 0; i < tupleDB.Count; i++)
                {
                    if (tupleDB[i].Item1 != Directions.Empty)
                    {
                        if (tupleDB[i].Item2.CollisionBlockType == CollisionType.Unthrougthable)
                        {
                            player.SetDirectionBlocker((int)tupleDB[i].Item1, false);
                        }
                        else if(tupleDB[i].Item2.CollisionBlockType == CollisionType.Througthable)
                        {
                            if (tupleDB[i].Item2.ObjectType == ObjectType.IceGround)
                            {
                                player.GameObject.TemporarySpeedUp();
                            }
                            else if(tupleDB[i].Item2.ObjectType == ObjectType.Swamp)
                            {
                                player.GameObject.TemporarySpeedDown();
                            }
                            else if(tupleDB[i].Item2.ObjectType == ObjectType.Armor 
                                || tupleDB[i].Item2.ObjectType == ObjectType.SpeedUp 
                                || tupleDB[i].Item2.ObjectType == ObjectType.ProjectileSpeed)
                            {
                                ((Bonus)tupleDB[i].Item2).ProccessEffect(player);
                                _sceneGenerator.RemoveGameobject(tupleDB[i].Item2);
                            }
                            else
                            {
                                player.GameObject.ResetSpeed();
                            }
                        }
                    }
                }
            }
            else
            {
                player.GameObject.ResetSpeed();
            }
        }

        private void OnlockDirectionBlocker(Player player)
        {
            for (int i = 0; i < player.GetLengthBlocker(); i++)
            {
                player.SetDirectionBlocker(i, true);
            }
        }

        private void UpdateBlockProjectilesIntersection(Player player)
        {
            for(int i = 0; i< player.Projectiles.Length; ++i)
            {
                if (player.Projectiles[i].Display)
                {
                    List<Block> intersectedBlocks = Collision2D.CollisionDirectionsArrayPlS1(player.Projectiles[i], _sceneGenerator.GameObjects);
                    foreach (Block block in intersectedBlocks)
                    {
                        if (block.CollisionProjectileType == CollisionType.Unthrougthable && !block.Equals(player.GameObject))
                        {
                            //player.DisplayToggle(false);
                            //player.ProjectileGenLockerToggle(false);
                            player.ResetProjectilePosition(i);
                            if (block.ObjectType == ObjectType.Destructible)
                            {
                                if(((DestructibleObsctacle)block).RegHitPoint())
                                {
                                    _sceneGenerator.RemoveGameobject(block);
                                }
                                //goto IntersectionSkip;
                            }
                            else if(block.ObjectType == ObjectType.Tank)
                            {
                                ((Tank)block).UpdateArmor(-1);
                            }
                        }
                    }
                }
            }
        /*IntersectionSkip:
            return;*/
        }
    }
}
