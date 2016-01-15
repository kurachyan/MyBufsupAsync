using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRSkipAsync;

namespace BufsupAsync
{
    public class CS_BufsupAsync
    {
        #region 共有領域
        // '16.01.13 両側余白情報削除の追加　及び、右側・左側余白処理のコメント化
/*
        CS_RskipAsync rskip;             // 右側余白情報を削除
        CS_LskipAsync lskip;             // 左側余白情報を削除
*/
        CS_LRskipAsync lrskip;           // 両側余白情報を削除

        private String _wbuf;       // ソース情報
        private Boolean _empty;     // ソース情報有無
        private String _rem;        // 注釈情報
        private Boolean _remark;    // 注釈管理情報
        public String Wbuf
        {
            get
            {
                return (_wbuf);
            }
            set
            {
                _wbuf = value;
                if (_wbuf == null)
                {   // 設定情報は無し？
                    _empty = true;
                }
                else
                {   // 整形処理を行う
                    // 不要情報削除
/*
                    if (rskip == null || lskip == null)
                    {   // 未定義？
                        rskip = new CS_RskipAsync();
                        lskip = new CS_LskipAsync();
                    }
                    rskip.Wbuf = _wbuf;
                    rskip.ExecAsync();
                    lskip.Wbuf = rskip.Wbuf;
                    lskip.ExecAsync();
                    _wbuf = lskip.Wbuf;
*/
                    if (lrskip == null)
                    {   // 未定義？
                        lrskip = new CS_LRskipAsync();
                    }
                    lrskip.ExecAsync(_wbuf);
                    _wbuf = lrskip.Wbuf;

                    // 作業の為の下処理
                    if (_wbuf.Length == 0 || _wbuf == null)
                    {   // バッファー情報無し
                        // _wbuf = null;
                        _empty = true;
                    }
                    else
                    {
                        _empty = false;
                    }
                    _rem = null;
                }
            }
        }

        public String Rem
        {
            get
            {
                return (_rem);
            }
        }

        public Boolean Remark
        {
            get
            {
                return (_remark);
            }
        }
        #endregion

        #region コンストラクタ
        public CS_BufsupAsync()
        {   // コンストラクタ
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            lrskip = null;
        }
        #endregion

        #region モジュール
        public async Task ClearAsync()
        {   // 作業領域の初期化
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            lrskip = null;
        }
        public async Task ExecAsync()
        {   // 構文評価を行う
            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     
                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskipAsync();
                }

                do
                {
                    if (_judge == true)
                    {   // Rskip稼働？                      
                        await Reskip();          // Rskip・Lskip稼働
                        _judge = false;
                        if (_wbuf == null)
                        {   // 評価対象が存在しない？
                            break;
                        }
                    }

                    _pos = _wbuf.IndexOf(@"//");
                    if (_pos != -1)
                    {   // コメント"//"検出？
                        await Supsub(_pos);
                        break;
                    }

                    _pos = _wbuf.IndexOf(@"/*");
                    if (_pos != -1)
                    {   // コメント"/*"検出？
                        await Supsub(_pos);
                        _remark = true;         // コメント開始
                        _judge = true;          // Rskip稼働
                    }

                    _pos = _wbuf.IndexOf(@"*/");
                    if (_pos != -1)
                    {   // コメント"*/"検出？
                        await RSupsub(_pos);
                        _remark = false;        // コメント終了
                        _judge = true;          // Rskip稼働
                    }

                    if (_rem != null)
                    {   // コメント設定有り？
                        _pos = _rem.IndexOf(@"*/");
                        if (_pos != -1)
                        {   // コメント"*/"検出？
                            await RRSupsub(_pos);
                            _remark = false;        // コメント終了
                            _judge = true;          // Rskip稼働
                        }

                    }
                } while (_pos > 0);

                await Reskip();              // Rskip稼働
                if (_wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }

            }

        }

        public async Task ExecAsync(String msg)
        {   // 構文評価を行う
            await SetbufAsync(msg);                 // 入力内容の作業領域設定

            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     

                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskipAsync();
                }

                do
                {
                    if (_judge == true)
                    {   // Rskip稼働？                      
                        await Reskip();          // Rskip・Lskip稼働
                        _judge = false;
                        if (_wbuf == null)
                        {   // 評価対象が存在しない？
                            break;
                        }
                    }

                    _pos = _wbuf.IndexOf(@"//");
                    if (_pos != -1)
                    {   // コメント"//"検出？
                        await Supsub(_pos);
                        break;
                    }

                    _pos = _wbuf.IndexOf(@"/*");
                    if (_pos != -1)
                    {   // コメント"/*"検出？
                        await Supsub(_pos);
                        _remark = true;         // コメント開始
                        _judge = true;          // Rskip稼働
                    }

                    _pos = _wbuf.IndexOf(@"*/");
                    if (_pos != -1)
                    {   // コメント"*/"検出？
                        await RSupsub(_pos);
                        _remark = false;        // コメント終了
                        _judge = true;          // Rskip稼働
                    }

                    if (_rem != null)
                    {   // コメント設定有り？
                        _pos = _rem.IndexOf(@"*/");
                        if (_pos != -1)
                        {   // コメント"*/"検出？
                            await RRSupsub(_pos);
                            _remark = false;        // コメント終了
                            _judge = true;          // Rskip稼働
                        }

                    }
                } while (_pos > 0);

                await Reskip();              // Rskip稼働
                if (_wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }

            }

        }
        #endregion

        #region サブ・モジュール
        private async Task Supsub(int __pos)
        {
            String __wbuf;  // コード情報

            __wbuf = _wbuf.Substring(0, __pos);      // コード抜き出し
            _rem += _wbuf.Substring(__pos + 2, _wbuf.Length - __pos - 2);  // コメント抜き出し
            _wbuf = __wbuf;

        }
        private async Task RSupsub(int __pos)
        {
            _rem += _wbuf.Substring(0, __pos + 2);  // コメント抜き出し
            _wbuf = _wbuf.Substring(__pos + 2, _wbuf.Length - __pos - 2);      // コード抜き出し

        }
        private async Task RRSupsub(int __pos)
        {
            String __rem;       //コメント情報

            __rem = _rem.Substring(0, __pos);  // コメント抜き出し
            _wbuf += _rem.Substring(__pos + 2, _rem.Length - __pos - 2);      // コード抜き出し
            _rem = __rem;

        }
        private async Task Reskip()
        {
/*
            rskip.Wbuf = _wbuf;
            await rskip.ExecAsync();
            lskip.Wbuf = rskip.Wbuf;
            await lskip.ExecAsync();
            _wbuf = lskip.Wbuf;
*/
            await lrskip.ExecAsync(_wbuf);
            _wbuf = lrskip.Wbuf;
        }
        private async Task SetbufAsync(String _strbuf)
        {   // [_wbuf]情報設定
            _wbuf = _strbuf;
            if (_wbuf == null)
            {   // 設定情報は無し？
                _empty = true;
            }
            else
            {   // 整形処理を行う
                // 不要情報削除
                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskipAsync();
                }
                await lrskip.ExecAsync(_wbuf);
                _wbuf = lrskip.Wbuf;

                // 作業の為の下処理
                if (_wbuf.Length == 0 || _wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }
                else
                {
                    _empty = false;
                }

                _rem = null;        // 注釈情報　初期化
            }
        }
        #endregion
    }
}
