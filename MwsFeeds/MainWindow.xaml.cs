using MarketplaceWebService;
using MarketplaceWebService.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace MwsFeeds
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// File UpLoad Proc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmitFeed_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            SubmitFeedRequest request = new SubmitFeedRequest();
            request.Merchant = SellerId;
            request.MarketplaceIdList = new IdList();
            request.MarketplaceIdList.Id = new List<string>(new string[] { MarketplaceId });
            request.MWSAuthToken = MWSAuthToken;

            request.FeedContent = System.IO.File.Open(txtFileName.Text, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
            request.FeedContent.Position = 0;
            request.FeedType = "_POST_FLAT_FILE_PRICEANDQUANTITYONLY_UPDATE_DATA_";
            SubmitFeedResponse response = client.SubmitFeed(request);

            if (response.IsSetSubmitFeedResult())
            {
                SubmitFeedResult submitFeedResult = response.SubmitFeedResult;
                if (submitFeedResult.IsSetFeedSubmissionInfo())
                {
                    FeedSubmissionInfo feedSubmissionInfo = submitFeedResult.FeedSubmissionInfo;
                    if (feedSubmissionInfo.IsSetFeedSubmissionId())
                    {
                        strbuff = "正常終了 レポートID：" + feedSubmissionInfo.FeedSubmissionId;
                    }
                    else
                    {
                        strbuff = "処理でエラーが発生しました。";
                    }
                }
            }
            txSubmitFeedResult.Text = strbuff;
        }

        /// <summary>
        /// Get SubmissionList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFeedSubmissionList_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            GetFeedSubmissionListRequest request = new GetFeedSubmissionListRequest();
            request.Merchant = SellerId;
            request.MWSAuthToken = MWSAuthToken;
            GetFeedSubmissionListResponse response = client.GetFeedSubmissionList(request);
            if (response.IsSetGetFeedSubmissionListResult())
            {
                GetFeedSubmissionListResult getFeedSubmissionListResult = response.GetFeedSubmissionListResult;
                if (getFeedSubmissionListResult.IsSetHasNext())
                {
                    strbuff += "次のリストの有無：" + getFeedSubmissionListResult.HasNext + System.Environment.NewLine;
                }
                List<FeedSubmissionInfo> feedSubmissionInfoList = getFeedSubmissionListResult.FeedSubmissionInfo;
                foreach (FeedSubmissionInfo feedSubmissionInfo in feedSubmissionInfoList)
                {
                    if (feedSubmissionInfo.IsSetFeedProcessingStatus())
                    {
                        strbuff += "レポートID：" + feedSubmissionInfo.FeedSubmissionId + System.Environment.NewLine;
                    }
                }
            }
            txtGetFeedSubmissionList.Text = strbuff;
        }

        /// <summary>
        /// Get Submission List Next Information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFeedSubmissionListByNext_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;
            string strNextToken = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            GetFeedSubmissionListRequest request = new GetFeedSubmissionListRequest();
            request.Merchant = SellerId;
            request.MWSAuthToken = MWSAuthToken;
            GetFeedSubmissionListResponse response = client.GetFeedSubmissionList(request);
            if (response.IsSetGetFeedSubmissionListResult())
            {
                GetFeedSubmissionListResult getFeedSubmissionListResult = response.GetFeedSubmissionListResult;
                if (getFeedSubmissionListResult.IsSetHasNext())
                {
                    strNextToken = getFeedSubmissionListResult.NextToken;
                    GetFeedSubmissionListByNextTokenRequest request1 = new GetFeedSubmissionListByNextTokenRequest();
                    request1.Merchant = SellerId;
                    request1.NextToken = strNextToken;
                    request1.MWSAuthToken = MWSAuthToken;

                    GetFeedSubmissionListByNextTokenResponse response1 = client.GetFeedSubmissionListByNextToken(request1);
                    if (response1.IsSetGetFeedSubmissionListByNextTokenResult())
                    {
                        GetFeedSubmissionListByNextTokenResult getFeedSubmissionListByNextTokenResult = response1.GetFeedSubmissionListByNextTokenResult;
                        List<FeedSubmissionInfo> feedSubmissionInfoList = getFeedSubmissionListByNextTokenResult.FeedSubmissionInfo;
                        foreach (FeedSubmissionInfo feedSubmissionInfo in feedSubmissionInfoList)
                        {
                            if (feedSubmissionInfo.IsSetFeedProcessingStatus())
                            {
                                strbuff += "レポートID：" + feedSubmissionInfo.FeedSubmissionId + System.Environment.NewLine;
                            }
                        }
                    }
                }
            }
            txtGetFeedSubmissionListByNext.Text = strbuff;
        }

        /// <summary>
        /// Get Submission Count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetSubmissionCount_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            GetFeedSubmissionCountRequest request = new GetFeedSubmissionCountRequest();
            request.Merchant = SellerId;
            request.MWSAuthToken = MWSAuthToken;

            GetFeedSubmissionCountResponse response = client.GetFeedSubmissionCount(request);
            if (response.IsSetGetFeedSubmissionCountResult())
            {
                GetFeedSubmissionCountResult getFeedSubmissionCountResult = response.GetFeedSubmissionCountResult;
                if (getFeedSubmissionCountResult.IsSetCount())
                {
                    strbuff += "処理件数：" + getFeedSubmissionCountResult.Count;
                }
            }
            txtGetFeedSubmissionCount.Text = strbuff;
        }

        /// <summary>
        /// Cancel Submission Proc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelFeedSubmission_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            CancelFeedSubmissionsRequest request = new CancelFeedSubmissionsRequest();
            request.Merchant = SellerId;
            request.MWSAuthToken = MWSAuthToken;
            IdList submissionIdList = new IdList();
            submissionIdList.Id.Add(txtCancelFeedSubmissionsSearch.Text);
            request.FeedSubmissionIdList = submissionIdList;

            CancelFeedSubmissionsResponse response = client.CancelFeedSubmissions(request);
            if (response.IsSetCancelFeedSubmissionsResult())
            {
                CancelFeedSubmissionsResult cancelFeedSubmissionsResult = response.CancelFeedSubmissionsResult;
                if (cancelFeedSubmissionsResult.IsSetCount())
                {
                    List<FeedSubmissionInfo> feedSubmissionInfoList = cancelFeedSubmissionsResult.FeedSubmissionInfo;
                    foreach (FeedSubmissionInfo feedSubmissionInfo in feedSubmissionInfoList)
                    {
                        strbuff += "キャンセル処理：" + feedSubmissionInfo.FeedSubmissionId;
                    }
                }
            }
            txtCancelFeedSubmission.Text = strbuff;
        }

        /// <summary>
        /// Get Submission Feed Result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFeedSubmissionResult_Click(object sender, RoutedEventArgs e)
        {
            string SellerId = CommonValue.strMerchantId;
            string MarketplaceId = CommonValue.strMarketplaceId;
            string AccessKeyId = CommonValue.strAccessKeyId;
            string SecretKeyId = CommonValue.strSecretKeyId;
            string ApplicationVersion = CommonValue.strApplicationVersion;
            string ApplicationName = CommonValue.strApplicationName;
            string MWSAuthToken = CommonValue.strMWSAuthToken;
            string strbuff = string.Empty;

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = CommonValue.strServiceURL;

            MarketplaceWebServiceClient client = new MarketplaceWebServiceClient(
                                                             AccessKeyId,
                                                             SecretKeyId,
                                                             ApplicationName,
                                                             ApplicationVersion,
                                                             config);
            GetFeedSubmissionResultRequest request = new GetFeedSubmissionResultRequest();
            request.FeedSubmissionId = txtFeedSubmissionResultSearch.Text;
            request.Merchant = SellerId;
            request.MWSAuthToken = MWSAuthToken;
            request.FeedSubmissionResult = System.IO.File.Open(@"C:\tmp\SubmissionResult.xml", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);

            GetFeedSubmissionResultResponse response = client.GetFeedSubmissionResult(request);
            if (response.IsSetGetFeedSubmissionResultResult())
            {
                strbuff = "処理が正常に完了しました。";
            }
            txtGetFeedSubmissionResult.Text = strbuff;
        }
    }
}
