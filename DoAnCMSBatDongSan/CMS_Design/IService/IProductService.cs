﻿using CloudinaryDotNet;
using CMS_Design.Handler.HandlePagination;
using CMS_Design.Payloads.DTOs.DataProductInformation;
using CMS_Design.Payloads.DTOs.DataResponseGeocoding;
using CMS_Design.Payloads.DTOs.DataResponseListProductSoldAndPrice;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany.StatisticsByMonth;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany.StatisticsByWeek;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using CMS_Design.Payloads.DTOs.DataResponseProductSoldStaffCommission;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndNumberOfPassengers;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByMonth;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByWeek;
using CMS_Design.Payloads.DTOs.SalesRatio;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByDay;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByMonth;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByWeek;
using CMS_Design.Payloads.Requests.ProductRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.IService
{
    public interface IProductService
    {
        Task<IQueryable<ProductStatisticsDTO>> GetStatisticalsProductOfManager(int pageSize, int pageNumber);
        Task<IQueryable<SalesAndProfitOfCompanyByMonth>> SalesStatisticsOfCompanyDuringMonth();
        Task<IQueryable<SalesAndProfitOfCompanyByWeek>> SalesStatisticsOfCompanyDuringWeek();
        Task<IQueryable<SalesStatisticsOfTeamDTO>> SalesStatisticsOfTeam();
        Task<IQueryable<SalesAndProfitOfTeamByWeek>> SalesStatisticsOfTeamDuringWeek();
        Task<IQueryable<SalesAndProfitOfTeamByMonth>> SalesStatisticsOfTeamDuringMonth();
        Task<IQueryable<SalesAndNumberOfPassengersByDayDTO>> ViewSalesStatisticsAndNumberOfPassengersByDay();
        Task<IQueryable<SalesAndNumberOfPassengersByMonthDTO>> ViewSalesStatisticsAndNumberOfPassengersByMonth();
        Task<IQueryable<SalesAndProfitOfCompanyAndTeam>> SalesRatioBetweenTeamAndCompany();
        Task<ResponseObject<ProductDTO>> AddNewProduct(int dauChuId, Request_AddNewProduct request);
        Task<ResponseObject<ProductDTO>> UpdateProduct(int productId,int dauChuId, Request_UpdateProduct request);
        Task<ResponseObject<ProductDTO>> AddProductImage(int productId, Request_CreateProductImg request);
        Task<IQueryable<StatisticsProductInformation>> GetStatisticsProductInformation();
        Task<PageResult<ProductDTO>> GetProductById(int? productId, int pageSize = 10, int pageNumber = 1);
        Task<PageResult<ProductDTO>> GetProductByName(string? name, int pageSize = 10, int pageNumber = 1);
        Task<PageResult<ProductDTO>> GetProductByAddress(string? address, int pageSize = 10, int pageNumber = 1);
        Task<PageResult<ProductDTO>> GetProductByOwner(int? ownerId, int pageSize = 10, int pageNumber = 1);
        Task<IQueryable<StatisticsAboutProductSoldStaffCommission>> StatisticsProductOfOwnerAndOrtherInfomation();
        Task<IQueryable<StatisticsSalesByDay>> GetStatisticsSalesByDay();
        Task<IQueryable<StatisticsSalesByWeek>> GetStatisticsSalesByWeek();
        Task<IQueryable<StatisticsSalesByMonth>> GetStatisticsSalesByMonth();
        Task<IQueryable<ProductNotYetSoldAndPrice>> GetProductSoldAndPrices();
        Task<IQueryable<ProductDTO>> GetAll(int pageSize, int pageNumber);
        Task<GeocodingResult> GetCoordinatesAsync(string address);
        Task<string> CreateRequestChangeStatusWhenClosingSuccessfully(int productId);
        Task<PageResult<ProductDTO>> GetAllProducts(int pageSize = 10, int pageNumber = 1);
        Task<ResponseObject<ProductDTO>> UpdateImageProduct(Request_UpdateImageProduct request);
    }
}
