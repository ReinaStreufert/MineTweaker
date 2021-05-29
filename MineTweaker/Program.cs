using MineTweaker.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Relay relay = new Relay();
            relay.RemoteServerHostname = "eu.hivemc.com";
            //relay.RemoteServerPort = 42069;
            relay.UseDefaultLoginHandler = true;

            Tweaker tweaker = new Tweaker();
            tweaker.RegisterCommand(new Hello());
            tweaker.RegisterCommand(new Speed());
            tweaker.RegisterCommand(new Jump());
            tweaker.RegisterCommand(new FallDamage());
            tweaker.RegisterCommand(new CanFly());
            tweaker.RegisterCommand(new SlowFalling());
            tweaker.RegisterCommand(new MineTweaker.Commands.Gamemode());
            relay.RegisterPacketHandler(tweaker);

            relay.Start();
            Console.ReadLine();
        }
    }
}
