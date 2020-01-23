using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using SupportPortal.Services;
using SupportPortal.Infrastructure;

namespace SupportPortal.Models
{
    public class DAO
    {
        private static readonly Common COMMON = new Common();

        private SqlConnection databaseConn { get; set; }
        public string query { get; set; }
        public string inhomeweek { get; set; }
        public string jobid { get; set; }
        public string newjobid { get; set; }
        public string sectionid { get; set; }
        public string statusid { get; set; }
        public string marketid { get; set; }
        public string formregionid { get; set; }
        public string uavc { get; set; }
        public string pageOrder { get; set; }
        public string ihd { get; set; }
        public string ovid { get; set; }
        public string pagecodes { get; set; }

        internal DataTable GetData()
        {
            databaseConn = null;
            DataTable dataTable = new DataTable();

            // Selects a query AND target database based on the query variable that is set form the controller
            string selectedQuery = EstablishQuery();
            dataTable.TableName = query;
            try
            {
                databaseConn.Open();

                // Executes based on the selected query and database from EstablishQuery()
                SqlCommand cmd = new SqlCommand(selectedQuery, databaseConn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);

                databaseConn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return dataTable;
        }


        internal String EstablishQuery()
        {
            string internalQuery;
            switch (query)
            {
                case "apdprintjob":
                    databaseConn = GetConnGSProdAuto();
                    internalQuery = "SELECT * FROM [GSProductAutomation].[DirectMail].[APD_DMPrintJobProcessing] WHERE [PageOrderName] IS NOT NULL ";
                    internalQuery += COMMON.IsEmptyOrNull(jobid) ? "" : "AND [JobID] LIKE '%" + jobid + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(uavc) ? "" : "AND [PageOrderName] LIKE '%" + uavc + "%' ";
                    return internalQuery;

                case "checkimportspider":
                    databaseConn = GetConnMistralAnn();
                    return "Select * from twistuser.check_import Order by import_start desc";

                case "dmmaster":
                    databaseConn = GetConnGSProdAuto();
                    internalQuery = "SELECT [InHomeWeek], [UAVC], [IHD], [OvID], [Complexity], [DTP], [FlagDoNotReleaseArt], [FlagJumpStart], [FuseRequestId], [GPON], [Hub], [JumpStartID], [PagePosition], [PreferredArtist], [PrintVendor], [PrintWeek], [TouchType], [VendorQuoteNumber], [LastActivity], [ReceivedTime], [Status], [Active], [Interval] FROM [GSProductAutomation].[DirectMail].[DirectMail] " +
                        "WHERE UAVC IS NOT NULL ";
                    internalQuery += COMMON.IsEmptyOrNull(inhomeweek) ? "" : "AND [InHomeWeek] LIKE '%" + inhomeweek + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(uavc) ? "" : "AND [UAVC] LIKE '%" + uavc + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(ihd) ? "" : "AND [IHD] LIKE '%" + ihd + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(ovid) ? "" : "AND [OvID] LIKE '%" + ovid + "%' ";
                    return internalQuery;

                case "dmgglog":
                    databaseConn = GetConnGSProdAuto();
                    internalQuery = "SELECT * FROM [GSProductAutomation].[DirectMail].[GraphicsGallery_Log] WHERE [FileName] IS NOT NULL ";
                    internalQuery += COMMON.IsEmptyOrNull(uavc) ? "" : "AND [FileName] LIKE '%" + uavc + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(inhomeweek) ? "" : "AND [IHW] LIKE '%" + inhomeweek + "%' ";
                    return internalQuery;

                case "dmprintvendor":
                    databaseConn = GetConnGSProdAuto();
                    internalQuery = "SELECT * FROM [GSProductAutomation].[DirectMail].[OutsidePrintVendor_Log] WHERE PageOrderName IS NOT NULL ";
                    internalQuery += COMMON.IsEmptyOrNull(uavc) ? "" : "AND PageOrderName LIKE '%" + uavc + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(inhomeweek) ? "" : "AND [IHW] LIKE '%" + inhomeweek + "%' ";
                    return internalQuery;

                case "fsimasthead":
                    databaseConn = GetConnPODPLA();
                    return String.Format("select" +
                        " i.job_id, f.form_region_id + f.Form_Market_id, i.region_id + i.market_id, Document_Title " +
                        "from InsertDocument i join FormnumXref f on i.job_id = f.Job_ID and i.region_id = f.Region_ID and i.Market_id = f.Market_ID " +
                        "where i.Job_ID = '{0}' " +
                        "and Form_Region_ID + Form_Market_ID = '{1}' " +
                        "order by Form_Region_ID + Form_Market_ID", jobid, formregionid + marketid);

                case "turnkeyprintjobprocessing":
                    databaseConn = GetConnGSProdAuto();
                    internalQuery = "SELECT * FROM [GSProductAutomation].[DirectMail].[APD_DMPrintJobProcessing] WHERE PageOrderName IS NOT NULL ";
                    internalQuery += COMMON.IsEmptyOrNull(pageOrder) ? "" : "AND PageOrderName LIKE '%" + pageOrder + "%' ";
                    internalQuery += COMMON.IsEmptyOrNull(jobid) ? "" : "AND JobID = " + jobid;
                    return internalQuery;

                case "turnkeylookup":
                    databaseConn = GetConnGSProdAuto();
                    string jobidRequest = jobid == null ? "" : "AND JobID = " + jobid;
                    return String.Format("select * from turnkey.AuditFilesReceived where UAVC = '{0}' {1}", uavc, jobidRequest);

                case "turnkeystatus":
                    databaseConn = GetConnGSProdAuto();
                    return "SELECT * FROM turnkey.Status";

                case "wrappagecodedetails":
                    databaseConn = GetConnGSProdAuto();
                    return String.Format("SELECT TOP(200) InHomeWeekNum, PageCode, UAVC, InHomeWeek, Client_Name, Coordinator, PagePosition, FC_Masthead, BC_Masthead, Billboard, Billboard_Client, Billboard_Coordinator, MissingChildImage, Printer, StatusID, StatusPCT, NumAttempts, NumRevisionsUAVC, NumRevisionsPageCode, UploadApproved, BillboardFileName, VersionFileName, PageCodeFileName, NewFileFlag, LastActivityOn, ApprovedOn, DateRequested, ProductFormat FROM  Wrap.PageCodeBuild WHERE(InHomeWeekNum = '{0}') AND (PageCode IN({1}))", inhomeweek, pagecodes);

                case "wrappagecodewaiting":
                    databaseConn = GetConnGSProdAuto();
                    return String.Format("select * from wrap.PageCodeBuild where InHomeWeekNum = '{0}' and PageCode IN ({1})", inhomeweek, pagecodes);

                case "wrapdoubles":
                    databaseConn = GetConnGSProdAuto();
                    return String.Format("SELECT COUNT(*), PageCode from wrap.PageCodeBuild where InHomeWeekNum = '{0}' group by PageCode having COUNT(*) > 1", inhomeweek);

                case "wrapdbstatus":
                    databaseConn = GetConnGSProdAuto();
                    return "SELECT * FROM wrap.PageCodeBuildStatus";

                case "spider":
                    databaseConn = GetConnMistralAnn();
                    string sectionidRequest = sectionid == null ? "" : "And SectionID = " + sectionid;
                    return String.Format("SELECT * FROM SpiderJDF WHERE JobID = {0} {1}", jobid, sectionidRequest);

                case "spiderimposition":
                    databaseConn = GetConnMistralAnn();
                    sectionidRequest = sectionid == null ? "" : "And SectionID = " + sectionid;
                    return String.Format("SELECT * FROM SpiderJDF_Imposition WHERE JobID = {0} {1}", jobid, sectionidRequest);

                case "livetodev":
                    try
                    {
                        databaseConn = GetConnMistralAnnDev();
                        databaseConn.Open();
                        internalQuery = String.Format("Insert Into SpiderJDf Select JobID, JobName, SiteName, ShipDate, DueDate, " +
                            "CustomerID, SectionID, SectionName, SectionFilterType, [PageCount], APRID, " +
                            "APRName, PageNum, CreativeID, OfferID, TrimHeight, TrimWidth, ImageHeight, " +
                            "ImageWidth, PageRotation, PaginationNum, PlatePositions, PressNumber, Comments, " +
                            "RegionFileName, [UID], EsAction, ExtractedDate " +
                            "from VALVCSSQ027VM.Mistral_Annex.dbo.SpiderJDf where jobID = '{0}'", jobid);

                        SqlCommand cmd = new SqlCommand(internalQuery, databaseConn);
                        cmd.ExecuteNonQuery();

                        internalQuery = String.Format("Insert into SpiderJDf_Imposition Select JobID, JobName, " +
                            "SectionID, ComboID, DisplayName, SectionArea, PressNumber, MarkLayout, " +
                            "PressClass, PaginationNum, RollSize, EsAction, ExtractedDate, MarginClass, " +
                            "Book_Quantity, StockType, OnPressDate, PrintLocation, SpineRow1, SpineRow2, " +
                            "SpineRow3 " +
                            "from VALVCSSQ027VM.Mistral_Annex.dbo.SpiderJDf_Imposition where jobID = '{0}'", jobid);

                        cmd = new SqlCommand(internalQuery, databaseConn);
                        cmd.ExecuteNonQuery();

                        internalQuery = String.Format("Insert into SpiderJDf_UniquePageDetails Select UID, JobID, PositionX, " +
                            "PositionY, CropW, CropH, OffsetX, OffsetY, CreativeFileName, " +
                            "AdFormatName " +
                            "from VALVCSSQ027VM.Mistral_Annex.dbo.SpiderJDf_UniquePageDetails where jobID = '{0}'", jobid);

                        cmd = new SqlCommand(internalQuery, databaseConn);
                        cmd.ExecuteNonQuery();

                        WebService web = new WebService();
                        web.KickSpider(jobid);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    databaseConn.Close();

                    return null;

                case "newlivejob":
                    databaseConn = GetConnMistralAnn();

                    return String.Format("Insert Into SpiderJDf Select '{1}', '{1}', SiteName, ShipDate, DueDate, " +
                        "CustomerID, SectionID, SectionName, SectionFilterType, [PageCount], APRID, APRName, PageNum, " +
                        "CreativeID, OfferID, TrimHeight, TrimWidth, ImageHeight, ImageWidth, PageRotation, PaginationNum, " +
                        "PlatePositions, PressNumber, Comments, RegionFileName, [UID], EsAction, " +
                        "ExtractedDate from SpiderJDf where jobID = '{0}' " +
                        "Insert into SpiderJDf_Imposition Select '{1}', '{1}', " +
                        "SectionID, ComboID, DisplayName, SectionArea, PressNumber, MarkLayout, PressClass, " +
                        "PaginationNum, RollSize, EsAction, ExtractedDate, MarginClass, Book_Quantity, StockType, " +
                        "OnPressDate, PrintLocation, SpineRow1, SpineRow2, SpineRow3 from " +
                        "SpiderJDf_Imposition where jobID = '{0}' " +
                        "Insert into SpiderJDf_UniquePageDetails Select UID, '{1}', PositionX, PositionY, CropW, CropH, " +
                        "OffsetX, OffsetY, CreativeFileName, AdFormatName from SpiderJDf_UniquePageDetails where " +
                        "jobID = '{0}'", jobid, newjobid);

                case "processexec":
                    databaseConn = GetConnPOD();
                    return "SELECT TOP 500 [ProcessExecutionID], [InsertDate], [FormNumber], " +
                        "[Stuffer], [ProcessID], [StartTime], [FinishTime], [StartedBy], [Parameter], " +
                        "[ReportDataBuilt], [SectionMetadataCompleted] " +
                        "FROM[Placement_OnDemand].[Support].[ProcessExecution] " +
                        "ORDER BY StartTime DESC";

                default:
                    return null;

            }
        }

        

        internal bool deleteData(string[] ids)
        {
            if (ids != null)
            {
                try
                {
                    // Takes the final element, which should be the query name. This will be used in the following switch statement
                    string query = ids[ids.Length - 1];

                    // Remove query from the end of ids, so that when we pass it in, it will only have the records intended for deletion
                    ids = ids.Where(x => x != query).ToArray();


                    // Selects a query AND target database based on the query variable that is set
                    switch (query)
                    {
                        case "spider":
                            databaseConn = GetConnMistralAnn();
                            query = "DELETE FROM [Mistral_Annex].[dbo].[SpiderJDF] WHERE RecordID IN (" + string.Join(",", ids) + ")";
                            break;

                        case "spiderimposition":
                            databaseConn = GetConnMistralAnn();
                            query = "DELETE FROM [Mistral_Annex].[dbo].[SpiderJDF_Imposition] WHERE RecordID IN (" + string.Join(",", ids) + ")";
                            break;

                        default:
                            query = null;
                            break;
                    }

                    try
                    {
                        databaseConn.Open();

                        SqlCommand cmd = new SqlCommand(query, databaseConn);
                        // Executes based on the selected query and database
                        cmd.ExecuteNonQuery();

                        databaseConn.Close();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return true;

        }



        // Connection objects, individually pulled based on need
        #region Database Connections
        private SqlConnection GetConnMistralAnn()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Mistral_Annex"].ConnectionString);
        }

        private SqlConnection GetConnMistralAnnDev()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Mistral_Annex_Dev"].ConnectionString);
        }

        private SqlConnection GetConnGSProdAuto_Dev()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["GSProductAutomation_Dev"].ConnectionString);
        }

        private SqlConnection GetConnGSProdAuto()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["GSProductAutomation"].ConnectionString);
        }

        private SqlConnection GetConnPODPLA()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["PODPLA"].ConnectionString);
        }

        private SqlConnection GetConnPOD_Dev()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Placement_OnDemand_Dev"].ConnectionString);
        }

        private SqlConnection GetConnPOD()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Placement_OnDemand"].ConnectionString);
        }
        #endregion

        public async Task<int> AuditProcess(string category, string process, string operation)
        {
            databaseConn = GetConnPOD_Dev();

            String query = "INSERT INTO dbo.SupportPortalAudit (Category,Process,Operation) VALUES (@category,@process, @operation)";

            using (SqlCommand cmd = new SqlCommand(query, databaseConn))
            {
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@process", process);
                cmd.Parameters.AddWithValue("@operation", operation);

                databaseConn.Open();
                int result = cmd.ExecuteNonQuery();

                // Check Error
                if (result < 0)
                    Console.WriteLine("Error inserting data into Database!");

                databaseConn.Close();
            }

            return await Task.FromResult(0);
        }

    }
}