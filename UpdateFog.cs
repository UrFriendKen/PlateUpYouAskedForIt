using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public class UpdateFog : DaySystem, IModSystem
    {
        EntityQuery Fog;
        EntityQuery Players;

        protected override void Initialise()
        {
            base.Initialise();
            Fog = GetEntityQuery(typeof(CFog), typeof(CPosition));
            Players = GetEntityQuery(typeof(CPlayer), typeof(CPosition));

            RequireForUpdate(Fog);
        }

        protected override void OnUpdate()
        {
            using NativeArray<CPlayer> players = Players.ToComponentDataArray<CPlayer>(Allocator.Temp);
            using NativeArray<CPosition> playerPositions = Players.ToComponentDataArray<CPosition>(Allocator.Temp);

            using NativeArray<Entity> fogEntities = Fog.ToEntityArray(Allocator.Temp);
            using NativeArray<CFog> fogs = Fog.ToComponentDataArray<CFog>(Allocator.Temp);
            using NativeArray<CPosition> fogPositions = Fog.ToComponentDataArray<CPosition>(Allocator.Temp);
            for (int i = 0; i < fogEntities.Length; i++)
            {
                Entity entity = fogEntities[i];
                CFog fog = fogs[i];
                CPosition fogPosition = fogPositions[i];

                fog.Enabled = Main.PrefManager.Get<bool>(Main.FOG_OF_WAR_ID);
                Set(entity, fog);

                if (!fog.Enabled || !RequireBuffer(entity, out DynamicBuffer<CFogInRangePlayer> fogPlayers))
                    continue;

                if (!TryGetPlayersInRange(fogPosition, fog.SameRoomRevealDist, fog.AlwaysRevealDist, out List<int> inputSources))
                {
                    fogPlayers.Clear();
                    continue;
                }

                List<int> fogPlayersAlreadyInRange = new List<int>();
                for (int j = fogPlayers.Length - 1; j >= 0; j--)
                {
                    if (!inputSources.Contains(fogPlayers[j].InputSource))
                        fogPlayers.RemoveAt(j);
                    else
                        fogPlayersAlreadyInRange.Add(fogPlayers[j].InputSource);
                }

                foreach (int inputSource in inputSources)
                {
                    if (!fogPlayersAlreadyInRange.Contains(inputSource))
                    {
                        fogPlayers.Add(new CFogInRangePlayer()
                        {
                            InputSource = inputSource
                        });
                    }
                }
            }

            bool TryGetPlayersInRange(Vector3 position, float sameRoomRevealDistance, float alwaysRevealDistance, out List<int> inputSources)
            {
                inputSources = new List<int>();
                float sqrRevealDistance = sameRoomRevealDistance * sameRoomRevealDistance;
                float sqrAlwaysDistance = alwaysRevealDistance * alwaysRevealDistance;
                int fogRoom = GetRoom(position);
                for (int i = 0; i < players.Length; i++)
                {
                    CPosition playerPosition = playerPositions[i];
                    float sqrDist = (playerPosition - position).sqrMagnitude;
                    int playerRoom = GetRoom(playerPosition);
                    if ((sqrDist < sqrRevealDistance && fogRoom == playerRoom) || sqrDist < sqrAlwaysDistance)
                    {
                        inputSources.Add(players[i].InputSource);
                    }
                }
                return inputSources.Count > 0;
            }
        }
    }
}
