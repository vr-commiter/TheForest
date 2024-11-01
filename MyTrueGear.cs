using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using TrueGearSDK;
using MelonLoader;
using System.Linq;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        private static ManualResetEvent heartbeatMRE = new ManualResetEvent(false);
        private static ManualResetEvent hungerMRE = new ManualResetEvent(false);
        private static ManualResetEvent shiverMRE = new ManualResetEvent(false);
        private static ManualResetEvent rainingMRE = new ManualResetEvent(false);
        private static ManualResetEvent divingMRE = new ManualResetEvent(false);
        private static ManualResetEvent planefallMRE = new ManualResetEvent(false);


        public void HeartBeat()
        {
            while (true)
            {
                heartbeatMRE.WaitOne();
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("HeartBeat");
                _player.SendPlay("HeartBeat");
                Thread.Sleep(600);
            }            
        }

        public void Hunger()
        {
            while (true)
            {
                hungerMRE.WaitOne();
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("Hunger");
                _player.SendPlay("Hunger");
                Thread.Sleep(500);
            }
        }

        public void Diving()
        {
            while (true)
            {
                divingMRE.WaitOne();
                _player.SendPlay("Diving");
                Thread.Sleep(1000);
            }            
        }

        public void PlaneFall()
        {
            while (true)
            {
                planefallMRE.WaitOne();
                _player.SendPlay("PlaneFall");
                Thread.Sleep(4000);
            }
            
        }

        public void Shiver()
        {
            while (true)
            {
                shiverMRE.WaitOne();
                int[,] ele = new int[,] { { 0, 1, 4, 5, 8, 9, 12, 13, 16, 17 }, { 2, 3, 6, 7, 110, 11, 14, 15, 18, 19 }, { 100, 101, 104, 105, 108, 109, 112, 113, 116, 117 }, { 102, 103, 106, 107, 110, 111, 114, 115, 118, 119 } };
                int[] random = new int[] { 1, 1, 1, 1 };
                PlayRandom("Shiver", ele, random);
                Thread.Sleep(100);
            }
            
        }

        public void Raining()
        {
            while (true)
            {
                rainingMRE.WaitOne();
                int[,] ele = new int[,] { { 0, 1, 4, 5 }, { 2, 3, 6, 7 }, { 100, 101, 104, 105 }, { 102, 103, 106, 107 } };
                int[] random = new int[] { 1, 1, 1, 1 };
                PlayRandom("Raining", ele, random);
                Thread.Sleep(200);
            }
            
        }

        public TrueGearMod() 
        {
            _player = new TrueGearPlayer("242760","The Forest VR");
            _player.PreSeekEffect("PhysicalDamage");
            _player.PreSeekEffect("ExplosionDamage");
            _player.Start();
            new Thread(new ThreadStart(this.HeartBeat)).Start();
            new Thread(new ThreadStart(this.Hunger)).Start();
            new Thread(new ThreadStart(this.Diving)).Start();
            new Thread(new ThreadStart(this.PlaneFall)).Start();
            new Thread(new ThreadStart(this.Shiver)).Start();
            new Thread(new ThreadStart(this.Raining)).Start();
        }


        public void Play(string Event)
        { 
            _player.SendPlay(Event);
        }

        public void PlayAngle(string tmpEvent, float tmpAngle, float tmpVertical)
        {
            try
            {
                float angle = (tmpAngle - 22.5f) > 0f ? tmpAngle - 22.5f : 360f - tmpAngle;
                int horCount = (int)(angle / 45) + 1;

                int verCount = tmpVertical > 0.1f ? -4 : tmpVertical < 0f ? 8 : 0;

                EffectObject oriObject = _player.FindEffectByUuid(tmpEvent);
                EffectObject rootObject = EffectObject.Copy(oriObject);
                foreach (TrackObject track in rootObject.trackList)
                {
                    if (track.action_type == ActionType.Shake)
                    {
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            if (verCount != 0)
                            {
                                track.index[i] += verCount;
                            }
                            if (horCount < 8)
                            {
                                if (track.index[i] < 50)
                                {
                                    int remainder = track.index[i] % 4;
                                    if (horCount <= remainder)
                                    {
                                        track.index[i] = track.index[i] - horCount;
                                    }
                                    else if (horCount <= (remainder + 4))
                                    {
                                        var num1 = horCount - remainder;
                                        track.index[i] = track.index[i] - remainder + 99 + num1;
                                    }
                                    else
                                    {
                                        track.index[i] = track.index[i] + 2;
                                    }
                                }
                                else
                                {
                                    int remainder = 3 - (track.index[i] % 4);
                                    if (horCount <= remainder)
                                    {
                                        track.index[i] = track.index[i] + horCount;
                                    }
                                    else if (horCount <= (remainder + 4))
                                    {
                                        var num1 = horCount - remainder;
                                        track.index[i] = track.index[i] + remainder - 99 - num1;
                                    }
                                    else
                                    {
                                        track.index[i] = track.index[i] - 2;
                                    }
                                }
                            }
                        }
                        if (track.index != null)
                        {
                            track.index = track.index.Where(i => !(i < 0 || (i > 19 && i < 100) || i > 119)).ToArray();
                        }
                    }
                    else if (track.action_type == ActionType.Electrical)
                    {
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            if (horCount <= 4)
                            {
                                track.index[i] = 0;
                            }
                            else
                            {
                                track.index[i] = 100;
                            }
                            if (horCount == 1 || horCount == 8 || horCount == 4 || horCount == 5)
                            {
                                track.index = new int[2] { 0, 100 };
                            }
                        }
                    }
                }
                _player.SendPlayEffectByContent(rootObject);
            }
            catch(Exception ex)
            { 
                MelonLogger.Msg("TrueGear Mod PlayAngle Error :" + ex.Message);
                _player.SendPlay(tmpEvent);
            }          
        }

        public void PlayRandom(string tmpEvent, int[,] tmpEletricals, int[] tmpRandomCounts)
        {
            try
            {
                var randomCounts = tmpRandomCounts;
                var eletricals = tmpEletricals;
                Random rand = new Random();

                int randomSum = 0;
                foreach (var random in randomCounts)
                {
                    randomSum += random;
                }

                EffectObject rootObject = _player.FindEffectByUuid(tmpEvent);

                var columns = eletricals.GetLength(1);
                foreach (TrackObject track in rootObject.trackList)
                {
                    if (track.action_type == ActionType.Shake)
                    {
                        while (track.index.Length < randomSum)
                        {
                            track.index.AddFirst(0);
                        }
                        int j = 0;
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            if (randomCounts[j] > 1)
                            {
                                int randomCol = rand.Next(columns);
                                while (eletricals[j, randomCol] == -1)
                                {
                                    randomCol = rand.Next(columns);
                                }
                                track.index[i] = eletricals[j, randomCol];
                                eletricals[j, randomCol] = -1;
                                randomCounts[j] -= 1;
                            }
                            else
                            {
                                int randomCol = rand.Next(columns);
                                while (eletricals[j, randomCol] == -1)
                                {
                                    randomCol = rand.Next(columns);
                                }
                                track.index[i] = eletricals[j, randomCol];
                                eletricals[j, randomCol] = -1;
                                randomCounts[j] -= 1;
                                j++;
                            }
                        }
                    }
                    else if (track.action_type == ActionType.Electrical)
                    {
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            int index = (int)rand.Next(0, 2);
                            if (index == 0)
                            {
                                track.index[i] = index;
                            }
                            else
                            {
                                track.index[i] = 100;
                            }
                        }
                    }
                }

                _player.SendPlayEffectByContent(rootObject);
            }
            catch (Exception ex) 
            {
                MelonLogger.Msg("TrueGear Mod PlayRandom Error :" + ex.Message);
                _player.SendPlay(tmpEvent);
            }
            
        }

        public void StartHeartBeat()
        {
            heartbeatMRE.Set();
        }

        public void StopHeartBeat()
        {
            heartbeatMRE.Reset();
        }

        public void StartHunger()
        {
            hungerMRE.Set();
        }

        public void StopHunger()
        {
            hungerMRE.Reset();
        }

        public void StartShiver()
        {
            shiverMRE.Set();
        }

        public void StopShiver()
        {
            shiverMRE.Reset();
        }

        public void StartRaining()
        {
            rainingMRE.Set();
        }

        public void StopRaining()
        {
            rainingMRE.Reset();
        }

        public void StartDiving()
        {
            divingMRE.Set();
        }

        public void StopDiving()
        {
            divingMRE.Reset();
        }

        public void StartPlaneFall()
        {
            planefallMRE.Set();
        }

        public void StopPlaneFall()
        {
            planefallMRE.Reset();
        }
    }
}
