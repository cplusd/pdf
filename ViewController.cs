using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using CoreMedia;
using AVKit;
using AVFoundation;
namespace PDF2
{
    public partial class ViewController : UIViewController
    {
        List<string> DataSource;
        public ViewController(IntPtr handle) : base(handle)
        {
        }
        public ViewController()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //-----
            NSUrl url = NSBundle.MainBundle.GetUrlForResource("simon", "mp4");
            UIImageView imv = new UIImageView(new CGRect(0, 0, 300, 400));
            imv.Image = GetVideoFirstViewImage(url);
            imv.BackgroundColor = UIColor.Blue;
            imv.Layer.BorderColor = UIColor.Red.CGColor;
            imv.Layer.BorderWidth = 3;
            this.View.AddSubview(imv);

            //------


            return;
            InitTableView();
            InitData();
            LoadData();

        }


        #region

        UIImage GetVideoFirstViewImage(NSUrl path)
        {
            UIImage videoImage=null;
            AVUrlAsset asset = new AVUrlAsset(path);
            AVAssetImageGenerator assetGen = new AVAssetImageGenerator(asset);
            assetGen.AppliesPreferredTrackTransform = true;
            CMTime time = CMTime.FromSeconds(0.0, 600);
            CMTime acturalTime;
            NSError error;
            CGImage image = assetGen.CopyCGImageAtTime(time, out acturalTime, out error);
            if(image!=null)
            {
                videoImage = UIImage.FromImage(image);
            }

            if(error!=null)
            {
                Console.WriteLine("error: " + error.Description);
            }
            if(image!=null)
                image.Dispose();

            return videoImage;

        }






        #endregion




        private void InitData()
        {
            DataSource = new List<string>() {
            "nixiang",
            "220_high_performance_auto_layout",
            "406_optimizing_app_startup_time",
            "415_behind_the_scenes_of_the_xcode_build_process"
            };


        }

        private void LoadData()
        {
            var souce= new TBS(DataSource);
            souce.CellClicked += HandleCellClicked;
            tableView.Source = souce;
            tableView.ReloadData();

        }

        private void HandleCellClicked(string obj)
        {

            SecondeViewController second = new SecondeViewController(obj);
            this.NavigationController.PushViewController(second, true);

        }

        UITableView tableView;
        private void InitTableView()
        {
            tableView = new UITableView() { TranslatesAutoresizingMaskIntoConstraints = false };
            tableView.EstimatedRowHeight = 44;

            View.AddSubview(tableView);
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-100-[tableView]-|", NSLayoutFormatOptions.AlignAllBaseline, "tableView", tableView));
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-0-[tableView]-0-|", NSLayoutFormatOptions.AlignAllBaseline, "tableView", tableView));


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }


    public class TBS : UITableViewSource
    {
        List<string> DataSource;
        public TBS(List<string> _DataSource)
        {
            DataSource = _DataSource;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(TBC.CID) as TBC;

            if(null==cell)
            {
                cell = new TBC(TBC.CID);
            }
            cell.TextLabel.Text = DataSource[indexPath.Row];
            return cell;

        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {

            return DataSource.Count;
        }
        public Action<string> CellClicked;
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            CellClicked?.Invoke(DataSource[indexPath.Row]);
        }


    }
    public class TBC:UITableViewCell
    {
        public const string CID = "cid";
        public TBC(string cid):base(UITableViewCellStyle.Default,cid)
        {
             
        }
    }

}