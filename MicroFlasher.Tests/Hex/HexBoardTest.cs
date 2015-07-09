using MicroFlasher.Hex;
using NUnit.Framework;

namespace MicroFlasher.Tests.Hex {
    [TestFixture]
    public class HexBoardTest {

        [Test]
        public void SplitNoPagesTest() {
            var board = new HexBoard();
            board[0x10] = 0x50;
            board[0x20] = 0x10;
            board[0x30] = 0x20;

            var blocks = board.SplitBlocks(1);
            Assert.AreEqual(3, blocks.Blocks.Count);

            var firstBlock = blocks.Blocks[0];
            var secondBlock = blocks.Blocks[1];
            var thirdBlock = blocks.Blocks[2];

            Assert.AreEqual(0x10, firstBlock.Address);
            Assert.AreEqual(0x20, secondBlock.Address);
            Assert.AreEqual(0x30, thirdBlock.Address);

            Assert.AreEqual(new byte[] { 0x50 }, firstBlock.Data);
            Assert.AreEqual(new byte[] { 0x10 }, secondBlock.Data);
            Assert.AreEqual(new byte[] { 0x20 }, thirdBlock.Data);
        }

        [Test]
        public void SplitPagesTest() {
            var board = new HexBoard();
            board[0x04] = 0x50;
            board[0x10] = 0x10;
            board[0x17] = 0x20;

            var blocks = board.SplitBlocks(0x08);
            Assert.AreEqual(2, blocks.Blocks.Count);

            var firstBlock = blocks.Blocks[0];
            var secondBlock = blocks.Blocks[1];

            Assert.AreEqual(0x00, firstBlock.Address);
            Assert.AreEqual(0x10, secondBlock.Address);

            Assert.AreEqual(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x50, 0xff, 0xff, 0xff }, firstBlock.Data);
            Assert.AreEqual(new byte[] { 0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x20 }, secondBlock.Data);
        }

        [Test]
        public void SplitSuperPagesMaxTest() {
            var board = new HexBoard();
            board[0x04] = 0x50;
            board[0x0c] = 0x30;
            board[0x10] = 0x10;
            board[0x17] = 0x20;

            board[0x24] = 0xd0;
            board[0x2c] = 0xb0;
            board[0x30] = 0x90;
            board[0x37] = 0xa0;

            var blocks = board.SplitBlocks(0x08);
            Assert.AreEqual(2, blocks.Blocks.Count);

            var firstBlock = blocks.Blocks[0];
            var secondBlock = blocks.Blocks[1];

            Assert.AreEqual(0x00, firstBlock.Address);
            Assert.AreEqual(0x20, secondBlock.Address);

            Assert.AreEqual(new byte[] {
                0xff, 0xff, 0xff, 0xff, 0x50, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0x30, 0xff, 0xff, 0xff,
                0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x20
            }, firstBlock.Data);
            Assert.AreEqual(new byte[] {
                0xff, 0xff, 0xff, 0xff, 0xd0, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0xb0, 0xff, 0xff, 0xff,
                0x90, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xa0
            }, secondBlock.Data);
        }

        [Test]
        public void SplitSuperPagesTest() {
            var board = new HexBoard();
            board[0x04] = 0x50;
            board[0x0c] = 0x30;
            board[0x10] = 0x10;
            board[0x17] = 0x20;

            board[0x24] = 0xd0;
            board[0x2c] = 0xb0;
            board[0x30] = 0x90;
            board[0x37] = 0xa0;

            var blocks = board.SplitBlocks(0x08, 0x10);
            Assert.AreEqual(4, blocks.Blocks.Count);

            var firstBlock = blocks.Blocks[0];
            var secondBlock = blocks.Blocks[1];
            var thridBlock = blocks.Blocks[2];
            var forthBlock = blocks.Blocks[3];

            Assert.AreEqual(0x00, firstBlock.Address);
            Assert.AreEqual(0x10, secondBlock.Address);
            Assert.AreEqual(0x20, thridBlock.Address);
            Assert.AreEqual(0x30, forthBlock.Address);

            Assert.AreEqual(new byte[] {
                0xff, 0xff, 0xff, 0xff, 0x50, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0x30, 0xff, 0xff, 0xff,
            }, firstBlock.Data);
            Assert.AreEqual(new byte[] {
                0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x20,
            }, secondBlock.Data);
            Assert.AreEqual(new byte[] {
                0xff, 0xff, 0xff, 0xff, 0xd0, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0xb0, 0xff, 0xff, 0xff
            }, thridBlock.Data);
            Assert.AreEqual(new byte[] {
                0x90, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xa0,
            }, forthBlock.Data);
        }
    }
}
