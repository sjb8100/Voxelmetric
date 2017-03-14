﻿using System.Collections.Generic;
using Voxelmetric.Code.Utilities;

namespace Voxelmetric.Code.Common.Threading.Managers
{
    public static class IOPoolManager
    {
        private static readonly List<ITaskPoolItem> WorkItems = new List<ITaskPoolItem>(2048);

        public static void Add(ITaskPoolItem action)
        {
            WorkItems.Add(action);
        }

        public static void Commit()
        {
            // Commit all the work we have
            if (Utilities.Core.UseThreadedIO)
            {
                TaskPool pool = Globals.IOPool;

                for (int i = 0; i<WorkItems.Count; i++)
                {
                    pool.AddItem(WorkItems[i]);
                }

                // Remove processed work items
                WorkItems.Clear();
            }
            else
            {
                using (TimeBudgetHandler budget = new TimeBudgetHandler(10))
                {
                    int i;
                    for (i = 0; i<WorkItems.Count; i++)
                    {
                        budget.StartMeasurement();
                        WorkItems[i].Run();
                        budget.StopMeasurement();

                        // If the tasks take too much time to finish, spread them out over multiple
                        // frames to avoid performance spikes
                        if (!budget.HasTimeBudget)
                            break;
                    }

                    WorkItems.RemoveRange(0, i+1);
                }
            }
        }
    }
}
