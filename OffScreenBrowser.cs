﻿// Copyright(c) 2019 pypy. All rights reserved.
//
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using CefSharp;
using CefSharp.Enums;
using CefSharp.OffScreen;
using CefSharp.Structs;
using SharpDX.Direct3D11;
using System;

namespace VRCX
{
    public class OffScreenBrowser : ChromiumWebBrowser, IRenderHandler
    {
        private Texture2D _texture;

        public OffScreenBrowser(Texture2D texture, string address)
            : base(address, new BrowserSettings()
            {
                DefaultEncoding = "UTF-8"
            })
        {
            _texture = texture;
            Size = new System.Drawing.Size(texture.Description.Width, texture.Description.Height);
            RenderHandler = this;
            Util.ApplyJavascriptBindings(JavascriptObjectRepository);
        }

        public new void Dispose()
        {
            RenderHandler = null;
            base.Dispose();
            _texture = null;
        }

        //

        ScreenInfo? IRenderHandler.GetScreenInfo()
        {
            return new ScreenInfo
            {
                DeviceScaleFactor = 1f
            };
        }

        bool IRenderHandler.GetScreenPoint(int viewX, int viewY, out int screenX, out int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return false;
        }

        Rect IRenderHandler.GetViewRect()
        {
            return new Rect(0, 0, Size.Width, Size.Height);
        }

        void IRenderHandler.OnAcceleratedPaint(PaintElementType type, Rect dirtyRect, IntPtr sharedHandle)
        {
            // NOT USED
        }

        void IRenderHandler.OnCursorChange(IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
        {
        }

        void IRenderHandler.OnImeCompositionRangeChanged(Range selectedRange, Rect[] characterBounds)
        {
        }

        void IRenderHandler.OnPaint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            if (type == PaintElementType.View)
            {
                using (var device = _texture.Device)
                using (var context = device.ImmediateContext)
                {
                    var dataBox = context.MapSubresource(_texture, 0, MapMode.WriteDiscard, MapFlags.None);
                    if (dataBox.IsEmpty == false)
                    {
                        var sourcePtr = buffer;
                        var destinationPtr = dataBox.DataPointer;
                        var rowPitch = dataBox.RowPitch;
                        var pitch = width * 4;
                        if (rowPitch == pitch)
                        {
                            WinApi.CopyMemory(destinationPtr, sourcePtr, (uint)(width * height * 4));
                        }
                        else
                        {
                            for (var i = height; i > 0; --i)
                            {
                                WinApi.CopyMemory(destinationPtr, sourcePtr, (uint)pitch);
                                sourcePtr += pitch;
                                destinationPtr += rowPitch;
                            }
                        }
                    }
                    context.UnmapSubresource(_texture, 0);
                }
                /*_bufferAssignLock.EnterUpgradeableReadLock();
                try
                {
                    if (_buffer.IsAllocated == false ||
                        width != _width ||
                        height != _height)
                    {
                        _bufferAssignLock.EnterWriteLock();
                        try
                        {
                            _width = width;
                            _height = height;
                            _bufferSize = width * height * 4;
                            if (_buffer.IsAllocated == true)
                            {
                                _buffer.Free();
                            }
                            _buffer = GCHandle.Alloc(new byte[_bufferSize], GCHandleType.Pinned);
                        }
                        finally
                        {
                            _bufferAssignLock.ExitWriteLock();
                        }
                    }
                    WinApi.CopyMemory(_buffer.AddrOfPinnedObject(), buffer, (uint)_bufferSize);
                }
                finally
                {
                    _bufferAssignLock.ExitUpgradeableReadLock();
                }*/
            }

            /*_bufferAssignLock.EnterReadLock();
            try
            {
                if (_buffer.IsAllocated == true)
                {
                    var context = _texture.Device.ImmediateContext;
                    var box = context.MapSubresource(_texture, 0, MapMode.WriteDiscard, MapFlags.None);
                    var destinationPtr = box.DataPointer;
                    var sourcePtr = _buffer.AddrOfPinnedObject();
                    if (destinationPtr != IntPtr.Zero)
                    {
                        if (box.RowPitch == _width * 4)
                        {
                            WinApi.CopyMemory(destinationPtr, sourcePtr, (uint)_bufferSize);
                        }
                        else
                        {

                            var pitch = box.RowPitch;
                            var rowPitch = _width * 4;
                            for (var i = _height; i > 0; --i)
                            {
                                WinApi.CopyMemory(destinationPtr, sourcePtr, (uint)rowPitch);
                                destinationPtr += pitch;
                                sourcePtr += rowPitch;
                            }
                        }
                    }
                    context.UnmapSubresource(_texture, 0);
                }
            }
            finally
            {
                _bufferAssignLock.ExitReadLock();
            }*/
        }

        void IRenderHandler.OnPopupShow(bool show)
        {
        }

        void IRenderHandler.OnPopupSize(Rect rect)
        {
        }

        void IRenderHandler.OnVirtualKeyboardRequested(IBrowser browser, TextInputMode inputMode)
        {
        }

        bool IRenderHandler.StartDragging(IDragData dragData, DragOperationsMask mask, int x, int y)
        {
            return false;
        }

        void IRenderHandler.UpdateDragCursor(DragOperationsMask operation)
        {
        }
    }
}
