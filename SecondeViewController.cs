using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
namespace PDF2
{
    public class SecondeViewController:UIViewController
    {

        string pdfPath;
        nfloat space = 1;
        nint pages = 0;
        public SecondeViewController(string _pdfPath)
        {
            pdfPath = _pdfPath;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            ShowPdfByDraw();

            CGImage cgimg = PDFtoImage();
            UIImage img = new UIImage(cgimg);
            UIImageView imv = new UIImageView(new CGRect(0, 0, 300, 400));
            imv.Image = img;
            imv.BackgroundColor = UIColor.Blue;
            imv.Layer.BorderColor = UIColor.Red.CGColor;
            imv.Layer.BorderWidth = 3;
            this.View.AddSubview(imv);

        }




        #region 自己绘制pdf 
        nfloat scWidth = UIScreen.MainScreen.Bounds.Width;
        nfloat scHeight= UIScreen.MainScreen.Bounds.Height;         nfloat lastPosition_y = 0;         CGRect GetPageSize(CGPDFDocument gPDFDocument)         {             try             {                 CGPDFPage cGPDFPage = gPDFDocument.GetPage(1);                 CGRect mediaRect = cGPDFPage.GetBoxRect(CGPDFBox.Crop);                 return mediaRect;             }             catch (Exception e)             {                 Console.WriteLine("KK:" + e.Message);                 return CGRect.Empty;             }         }         class SCD : UIScrollViewDelegate         {             public UIView zoomV;             bool iszooming = false;             Action<object, EventArgs> HandleScrolled;             public SCD(Action<object, EventArgs> _HandleScrolled,UIView box)             {                 HandleScrolled = _HandleScrolled;                 zoomV = box;             }              public override void Scrolled(UIScrollView scrollView)             {                 if(iszooming)                 {                     return;                 }                 HandleScrolled.Invoke(null, null);             }             public override void ScrollAnimationEnded(UIScrollView scrollView)             {                 //Console.WriteLine("ScrollAnimationEnded");             }             public override void DecelerationEnded(UIScrollView scrollView)             {                 //Console.WriteLine("DecelerationEnded");                 //Console.WriteLine(zoomV.Subviews.Length);             }             public override UIView ViewForZoomingInScrollView(UIScrollView scrollView)             {                 return zoomV;             }             public override void ZoomingStarted(UIScrollView scrollView, UIView view)             {                            iszooming = true;             }             public override void ZoomingEnded(UIScrollView scrollView, UIView withView, nfloat atScale)             {                 iszooming = false;             }             public Action<nfloat> ZoomChanged;             public override void DidZoom(UIScrollView scrollView)             {                 ZoomChanged?.Invoke(scrollView.ZoomScale);                         }          }          private void HandleScrolled(object sender, EventArgs e)         {             nfloat position_y = sc.ContentOffset.Y;             if (position_y - lastPosition_y >= 15)             {                 if (showingIndex.Count == 1) return;                 //up +                                                     int index = showingIndex[1];                 nfloat top = (height + space)* zoomScale * (index - 1);                 if (position_y >= top)                 {                     int willbeAddedIndex = index + showingsPage - 1;                     if (willbeAddedIndex <= pages)                     {                         var vs = boxView.Subviews;                         vs[0].RemoveFromSuperview();                         showingIndex.RemoveAt(0);                         ShowOnePdfPageInSC(willbeAddedIndex, false);                         showingIndex.Add(willbeAddedIndex);                          UpdateCoverView(showingIndex[showingsPage-1]);                     }                                     }                 lastPosition_y = position_y;              }             else if (lastPosition_y - position_y >= 15)             {                 if (showingIndex.Count == 1) return;                 //down -                 int index = showingIndex[1];                 nfloat top = (height + space)* zoomScale * (index - 1);                 if (position_y <= top)                 {                     int willbeAddedIndex = showingIndex[0] - 1;                     if (willbeAddedIndex >= 1)                     {                         var vs = boxView.Subviews;                         vs[showingsPage - 1].RemoveFromSuperview();                         showingIndex.RemoveAt(showingsPage - 1);                         ShowOnePdfPageInSC(willbeAddedIndex, true);                         showingIndex.Insert(0, willbeAddedIndex);                         UpdateCoverView(showingIndex[showingsPage-1]);                     }                                    }                  lastPosition_y = position_y;             }          }         int showingsPage = 0;         List<int> showingIndex;         CGPDFDocument gPDFDocument;         UIScrollView sc;         CGRect pageSize;         nfloat height;//pdf height         nfloat zoomScale = 1;         UIView boxView;         private void HandleZoomChanged(nfloat obj)         {             zoomScale = obj;         }         UILabel cview;         void AddCoverView()         {              cview = new UILabel();             cview.TextAlignment = UITextAlignment.Center;             cview.BackgroundColor = UIColor.Orange;             cview.Frame = new CGRect(10,20,100,44);             cview.Layer.CornerRadius = 10;             cview.Layer.MasksToBounds = true;             this.View.AddSubview(cview);                            }         void UpdateCoverView(int index)         {             //cview.Text = index +"/" +pages;         }           public CGImage PDFtoImage()
        {
            CGPDFPage cGPDFPage = gPDFDocument.GetPage(1);
            var  pageSize = GetPageSize(gPDFDocument);
            CGContext outContext = CreateARGBBitmapContext((int)pageSize.Size.Width, (int)pageSize.Size.Height);
            if (outContext!=null)
            {
                outContext.DrawPDFPage(cGPDFPage);
                CGImage ThePDFImage = outContext.AsBitmapContext().ToImage();
                outContext.Dispose();
                return ThePDFImage;

            }
            return null;

        }                  CGContext CreateARGBBitmapContext(int pixelsWide, int pixelsHigh)
        {
            CGContext context = null;
            CGColorSpace colorSpace;            
            IntPtr bitmapData;

            int bitmapByteCount;

            int bitmapBytesPerRow;

            // Get image width, height. We’ll use the entire image.

            //  size_t pixelsWide = CGImageGetWidth(inImage);

            //  size_t pixelsHigh = CGImageGetHeight(inImage);

            // Declare the number of bytes per row. Each pixel in the bitmap in this

            // example is represented by 4 bytes; 8 bits each of red, green, blue, and

            // alpha.

            bitmapBytesPerRow = (pixelsWide * 4);

            bitmapByteCount = (bitmapBytesPerRow * pixelsHigh);

            // Use the generic RGB color space.            
            colorSpace = CGColorSpace.CreateWithName(CGColorSpaceNames.GenericRgb);
            if (colorSpace == null)
            {                       
                Console.WriteLine("Error allocating color space\n");
                return null;
            }
            // Allocate memory for image data. This is the destination in memory
            // where any drawing to the bitmap context will be rendered.
            
            bitmapData = Marshal.AllocHGlobal(bitmapByteCount);
             //bitmapData = malloc(bitmapByteCount);
            if (bitmapData == null)
            {                       
                Console.WriteLine("Memory not allocated!");
                //CGColorSpaceRelease(colorSpace);
                colorSpace.Dispose();
                return null;

            }

            // Create the bitmap context. We want pre-multiplied ARGB, 8-bits

            // per component. Regardless of what the source image format is

            // (CMYK, Grayscale, and so on) it will be converted over to the format

            // specified here by CGBitmapContextCreate.


            context = new CGBitmapContext(bitmapData, pixelsWide, pixelsHigh, 8, bitmapBytesPerRow, colorSpace, CGBitmapFlags.PremultipliedFirst);

            if (context == null)
            {                                        
                Marshal.FreeHGlobal(bitmapData);
                Console.WriteLine("Context not created!");

            }

            // Make sure and release colorspace before returning
            colorSpace.Dispose();                        
            return context;

        }
                             private void ShowPdfByDraw()         {             //elf.automaticallyAdjustsScrollViewInsets = false             this.AutomaticallyAdjustsScrollViewInsets = false;             sc = new UIScrollView(View.Bounds);             sc.ScrollsToTop = false;             sc.ShowsVerticalScrollIndicator = true;             sc.BouncesZoom = true;             sc.MaximumZoomScale = 8f;             sc.MinimumZoomScale = 1;             boxView = new UIView();             SCD selfDelegate = new SCD(HandleScrolled, boxView);             selfDelegate.ZoomChanged += HandleZoomChanged;             sc.Delegate = selfDelegate;             View.AddSubview(sc);             //AddCoverView();             int pageNumber = 1;                       NSUrl url = NSBundle.MainBundle.GetUrlForResource(pdfPath, "pdf");              gPDFDocument = CGPDFDocument.FromUrl(url.ToString());                         pages = gPDFDocument.Pages;                                        //获取page size             pageSize = GetPageSize(gPDFDocument);             nfloat scale = pageSize.Height / pageSize.Width;             height = scale * (scWidth - 2 * space);               int morePages = 6;             if (height>= scHeight)             {                 showingsPage = 1+ morePages;             }             else             {                 int a = (int)(Math.Ceiling(scHeight / height));                 showingsPage = a + morePages;                         }             if(showingsPage>=pages)             {                 showingsPage = (int)pages;             }             UpdateCoverView(1);              sc.AddSubview(boxView);             //AddCoverView();              showingIndex = new List<int>();             for (; pageNumber <= showingsPage; pageNumber++)              {                 ShowOnePdfPageInSC(pageNumber,false);                 showingIndex.Add(pageNumber);             }             boxView.Frame = new CGRect(0,0, scWidth, (height + space) * pages);             sc.ContentSize = new CGSize(scWidth, (height + space)*pages);                    }                void ShowOnePdfPageInSC(int pageNumber, bool isfront)         {             CGPDFPage cGPDFPage = gPDFDocument.GetPage(pageNumber);                          if (cGPDFPage == null)             {                 return;             }             CGRect mediaRect = cGPDFPage.GetBoxRect(CGPDFBox.Crop);             UIView pv = new UIView();             pv.Tag = pageNumber + 1000;             pv.Frame = new CGRect(space, space + (height + space) * (pageNumber - 1), scWidth - 2 * space, height);             var tiledLayer = new CATiledLayer
            {
                Frame = pv.Bounds
            };             var deleg = new CATiledLayerD(mediaRect, cGPDFPage);             deleg.drawLayerHapped += () => {                 //Console.WriteLine("绘制开始了："+zoomScale);             };             tiledLayer.Delegate = deleg;             tiledLayer.LevelsOfDetail = 1;             tiledLayer.LevelsOfDetailBias = 6;             pv.Layer.AddSublayer(tiledLayer);             if (isfront)             {                 boxView.InsertSubview(pv, 0);             }             else             {                              boxView.AddSubview(pv);             }          }         class CATiledLayerD : CALayerDelegate         {             CGRect mediaRect;             CGPDFPage cGPDFPage;             public Action drawLayerHapped;             public CATiledLayerD(CGRect _mediaRect, CGPDFPage _cGPDFPage)             {                 cGPDFPage = _cGPDFPage;                 mediaRect = _mediaRect;             }             public override void DrawLayer(CALayer layer, CGContext context)             {                 drawLayerHapped?.Invoke();                  var layer2 = layer as CATiledLayer;                 var bounds = layer.Bounds;                  context.TranslateCTM(bounds.X, bounds.Y);                 context.TranslateCTM(0, bounds.Height);                 nfloat sx = bounds.Size.Width / mediaRect.Size.Width;                 nfloat sy = -bounds.Size.Height / mediaRect.Size.Height;                 context.ScaleCTM(sx, sy);                 context.TranslateCTM(bounds.X, bounds.Y);                 context.InterpolationQuality = CGInterpolationQuality.High;                 context.SetRenderingIntent(CGColorRenderingIntent.Default);                 context.DrawPDFPage(cGPDFPage);              }         }          #endregion  




    }
}
