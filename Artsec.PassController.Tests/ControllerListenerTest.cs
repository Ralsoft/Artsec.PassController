using Artsec.PassController.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Tests
{
    public class ControllerListenerTest
    {
        [Fact]
        public async Task Test()
        {
            var src = new byte[]
            {
                0x04,
                0x01,
                0xA6,
                0xFE,
                0x2A,
                0xE7,
                0x1D,
                0xD4,
                0x05,
                0x1C,
                0xC4,
                0x9D,
                0x34,
                0x00,
                0x01,
                0xF6,
                0xF8,
            };
            var src2 = Encoding.UTF8.GetBytes("��*��ĝ4");
            var message = RfidMessage.Parse(src2);
            Encoding.UTF8.GetString(src);
        }
        [Fact]
        public async Task Test2()
        {
            var src = "04-01-A6-FE-2A-E7-1D-D4-05-1C-C4-9D-34-00-01-F6-F8";
            var a = Encoding.UTF8.GetBytes(src.Where(c => c != '-').ToArray());
            var message = BitConverter.ToString(a);
            Assert.Equal(src, message);
        }
    }
}
