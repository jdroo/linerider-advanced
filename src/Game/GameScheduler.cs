﻿//
//  GameScheduler.cs
//
//  Author:
//       Noah Ablaseau <nablaseau@hotmail.com>
//
//  Copyright (c) 2017 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;

namespace linerider
{
    public class GameScheduler
    {
        private Stopwatch sw = new Stopwatch();
        private double lastupdate = 0;
        private double updateperiod = 0;
        private bool reset = true;

        public int UpdatesPerSecond
        {
            get
            {
                return (int)(1.0 / updateperiod);
            }
            set
            {
                if (UpdatesPerSecond != value)
                {
                    updateperiod = 1.0 / value;
                    reset = true;
                }
            }
        }

        public int UnqueueUpdates()
        {
            if (updateperiod == 0) return 1;
            if (!sw.IsRunning)
                sw.Start();
            int updates = 0;
            var totalelapsed = sw.Elapsed.TotalSeconds;
            var elapsed = totalelapsed - lastupdate;
            if (reset)
            {
                reset = false;
                lastupdate = totalelapsed;
                return 1;
            }
            else
            {
                while (elapsed >= updateperiod)
                {
                    elapsed -= updateperiod;
                    updates++;
                    int cap = 3;
                    if (UpdatesPerSecond > 120)
                        cap = 10;
                    if (updates >= cap)
                    {
                        elapsed = Math.Min(elapsed, updateperiod * cap);
                        break;
                    }
                }
            }
            if (updates > 0)
                lastupdate = totalelapsed - elapsed;
            return updates;
        }

        public void Reset()
        {
            reset = true;
        }
    }
}