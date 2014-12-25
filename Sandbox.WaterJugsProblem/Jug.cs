using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.WaterJugsProblem
{
    /// <summary>
    /// The Jug class builds the objects that represent jugs in our problem.  They have a maximum and current
    /// volume, and methods for every action that can be performed on or with a jug in the problem.
    /// </summary>
    public class Jug
    {
        private int capacity;
        private int currentVolume = 0;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public int CurrentVolume 
        {
            get { return currentVolume; }
            set { currentVolume = value; }
        }

        public Jug (int jugCapacity)
        {
            capacity = jugCapacity;
        }

        public void Fill()
        {
            currentVolume = capacity;
        }

        public void Empty()
        {
            currentVolume = 0;
        }

        public void TransferTo(Jug otherJug)
        {
            int otherJugAvailableSpace = otherJug.Capacity - otherJug.CurrentVolume;
            if (otherJugAvailableSpace > currentVolume)
            {
                otherJug.CurrentVolume += currentVolume;
                currentVolume = 0;
            }
            else
            {
                currentVolume -= otherJugAvailableSpace;
                otherJug.Fill();
            }
        }
    }
}
