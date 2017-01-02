﻿// Copyright (c)  AG-Software. All Rights Reserved.
// by Alexander Gnauck (alex@ag-software.net)

using Matrix.Xml;
using Matrix.Xmpp.AdHocCommands;
using Matrix.Xmpp.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Matrix.Xmpp.Tests.AdHocCommands
{
   
    [TestClass]
    public class CommandTests
    {
        const string NOTE_VALUE = "Service 'httpd' has been configured.";
        private const string XML1 = @"<command xmlns='http://jabber.org/protocol/commands'
                       sessionid='config:20020923T213616Z-700'
                       node='config'
                       status='completed'>
                <note type='info'>" + NOTE_VALUE + @"</note>
              </command>";

        private const string XML2 = @"<command xmlns='http://jabber.org/protocol/commands'>                       
                <actions execute='next'>
                  <next/>
                </actions>        
              </command>";

        private const string XML3 = @"<command xmlns='http://jabber.org/protocol/commands' node='http://jabber.org/protocol/admin#add-user' action='execute' />";
        
        [TestMethod]
        public void Test1()
        {
            XmppXElement xmpp1 = XmppXElement.LoadXml(XML1);
            Assert.AreEqual(true, xmpp1 is Command);

            var cmd = xmpp1 as Command;
            if (cmd != null)
            {
                cmd.SessionId.ShouldBe("config:20020923T213616Z-700");
                cmd.Node.ShouldBe("config");
                cmd.Status.ShouldBe(Status.Completed);

                var note = cmd.Note;
                if (note != null)
                {
                    note.Type.ShouldBe(NoteType.Info);
                    note.Value.ShouldBe( NOTE_VALUE);
                    note.Value.ShouldNotBe("dummy");
                }
            }
        }

        [TestMethod]
        public void Test2()
        {
            var cmd = new Command
            {
                SessionId = "config:20020923T213616Z-700",
                Node = "config",
                Status = Status.Completed,
                Note = new Note
                {
                    Type = NoteType.Info,
                    Value = NOTE_VALUE
                }
            };

            cmd.ShouldBe(XML1);
        }


        [TestMethod]
        public void Test3()
        {
            XmppXElement xmpp1 = XmppXElement.LoadXml(XML2);
            Assert.AreEqual(true, xmpp1 is Command);

            var cmd = xmpp1 as Command;
            if (cmd != null)
            {
                var actions = cmd.Actions;
                if (actions != null)
                {
                    Assert.AreEqual(actions.Execute, Action.Next);
                    Assert.AreEqual(actions.Next, true);
                    Assert.AreEqual(actions.Previous, false);
                    Assert.AreEqual(actions.Action, Action.Next);
                    Assert.AreNotEqual(actions.Action, Action.Next | Action.Prev);

                    // modify actions now and test again
                    actions.Action = Action.Next | Action.Prev;
                    Assert.AreEqual(actions.Action, Action.Next | Action.Prev);
                    Assert.AreNotEqual(actions.Action, Action.Next | Action.Complete);
                }
            }
        }

        [TestMethod]
        public void Test4()
        {
            var cmd = new Command()
            {
                Node = "http://jabber.org/protocol/admin#add-user",
                Action = Action.Execute
            };

            cmd.ShouldBe(XML3);
        }

        [TestMethod]
        public void TestOnlineUsers()
        {
            string XML1 = @"<iq id='get-online-users-list-1'
                to='shakespeare.lit'
                type='set'
                xmlns='jabber:client'>
                <command xmlns='http://jabber.org/protocol/commands' 
                           action='execute'
                           node='http://jabber.org/protocol/admin#get-online-users-list'/>
                </iq>";

            var iq = new Iq
            {
                To = "shakespeare.lit",
                Id = "get-online-users-list-1",
                Type = Matrix.Xmpp.IqType.Set
            };

            var cmd = new Command()
            {
                Action = Action.Execute,
                Node = "http://jabber.org/protocol/admin#get-online-users-list"
            };

            iq.Add(cmd);

            iq.ShouldBe(XML1);
        }
    }
}