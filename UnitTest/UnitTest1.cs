﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BufsupAsync;


namespace UnitTest
{
    [TestClass]
    public class BufsupAsync_UnitTest1
    {
        [TestMethod]
        public void TestMethod11()
        {
            CS_BufsupAsync bufsup = new CS_BufsupAsync();

            #region 対象：コードのみ
            bufsup.ClearAsync();
            bufsup.Wbuf = @"This is a Pen.";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("This is a Pen.", bufsup.Wbuf, "Wbuf[This is a Pen.]");
            Assert.IsNull(bufsup.Rem);
            #endregion

            #region 対象：コメントのみ１
            bufsup.ClearAsync();
            bufsup.Wbuf = @"// This is a Pen.";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.IsNull(bufsup.Wbuf);
            Assert.AreEqual(" This is a Pen.", bufsup.Rem, "Rem[ This is a Pen.]");
            #endregion

            #region 対象：コメントのみ２
            bufsup.ClearAsync();
            bufsup.Wbuf = @"/* This is a Pen. */";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.IsNull(bufsup.Wbuf);
            Assert.AreEqual(" This is a Pen. ", bufsup.Rem, "Rem[ This is a Pen. ]");
            #endregion
        }

        [TestMethod]
        public void TestMethod12()
        {
            CS_BufsupAsync bufsup = new CS_BufsupAsync();

            #region 対象：コードとコメント１
            bufsup.ClearAsync();
            bufsup.Wbuf = @"This is a Pen.  // Test";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("This is a Pen.", bufsup.Wbuf, "Wbuf[This is a Pen.]");
            Assert.AreEqual(" Test", bufsup.Rem, "Rem[ Test]");
            #endregion

            #region 対象：コードとコメント２
            bufsup.ClearAsync();
            bufsup.Wbuf = @"This is a Pen.  /* Test */";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("This is a Pen.", bufsup.Wbuf, "Wbuf[This is a Pen.]");
            Assert.AreEqual(" Test ", bufsup.Rem, "Rem[ Test ]");
            #endregion

            #region 対象：コードとコメント３－１
            bufsup.ClearAsync();
            bufsup.Wbuf = @"This is a Pen.  /* Test";
            bufsup.ExecAsync();

            Assert.IsTrue(bufsup.Remark);
            Assert.AreEqual("This is a Pen.", bufsup.Wbuf, "Wbuf[This is a Pen.]");
            Assert.AreEqual(" Test", bufsup.Rem, "Rem[ Test]");
            #endregion

            #region 対象：コードとコメント３－２
            Assert.IsTrue(bufsup.Remark);
            bufsup.Wbuf = @"This is a Pen.  */ Test";
            bufsup.ExecAsync();

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("This is a Pen.  ", bufsup.Rem, "Rem[This is a Pen.  ]");
            Assert.AreEqual("Test", bufsup.Wbuf, "Wbuf[Test]");
            #endregion
        }
    }

    [TestClass]
    public class BufsupAsync_UnitTest2
    {
        [TestMethod]
        public void TestMethod21()
        {
            string[] Source = {
                @"/* Sample Code */",               // 0) Remark:False Rem[ Sample Code ]
                @"void main() {",                   // 1) Remark:False Wbuf[void main() {]
                @"     test1();  // test_Code",     // 2) Remark:False Wbuf[test1();] Rem[ test_Code]
                @"     /*",                         // 3) Remark:True Rem[]
                @"     test2();  // Comment_Code",  // 4) Remark:True Rem[     test2();  // Comment_Code]
                @"     */",                         // 5) Remark:False Rem[]
                @"}"                                // 6) Remark:False Wbuf[}]
            };

            CS_BufsupAsync bufsup = new CS_BufsupAsync();
            bufsup.ClearAsync();

            #region ソースコード０
//            bufsup.Wbuf = Source[0];
            bufsup.ExecAsync(Source[0]);

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual(" Sample Code ", bufsup.Rem, "Rem[ Sample Code ]");
            Assert.IsNull(bufsup.Wbuf);
            #endregion

            #region ソースコード１
//            bufsup.Wbuf = Source[1];
            bufsup.ExecAsync(Source[1]);

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("void main() {", bufsup.Wbuf, "Wbuf[void main() {]");
            Assert.IsNull(bufsup.Rem);
            #endregion

            #region ソースコード２
//            bufsup.Wbuf = Source[2];
            bufsup.ExecAsync(Source[2]);

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("test1();", bufsup.Wbuf, "Wbuf[test1();]");
            Assert.AreEqual(" test_Code", bufsup.Rem, "Rem[ test_Code]");
            #endregion

            #region ソースコード３
//            bufsup.Wbuf = Source[3];
            bufsup.ExecAsync(Source[3]);

            Assert.IsTrue(bufsup.Remark);
            #endregion

            #region ソースコード４
//            bufsup.Wbuf = Source[4];
            bufsup.ExecAsync(Source[4]);

            Assert.IsTrue(bufsup.Remark);
            #endregion

            #region ソースコード５
//            bufsup.Wbuf = Source[5];
            bufsup.ExecAsync(Source[5]);

            Assert.IsFalse(bufsup.Remark);
            #endregion

            #region ソースコード６
//            bufsup.Wbuf = Source[6];
            bufsup.ExecAsync(Source[6]);

            Assert.IsFalse(bufsup.Remark);
            Assert.AreEqual("}", bufsup.Wbuf, "Wbuf[}]");
            Assert.IsNull(bufsup.Rem);
            #endregion
        }
    }
}
