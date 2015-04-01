//---------------------------------------------------------------------------------
// <copyright file="AutomapperConfig.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.AppStart
{
    using System;
    using System.Linq;
    using AutoMapper;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Domain.Concrete;

    /// <summary>
    /// Initial configurations of Auto mapper
    /// </summary>
    public class AutomapperConfig
    {
        public static void AutoMapperConfig() 
        {
            Mapper.CreateMap<CWeigthScaleMessageNew, WeightScaleMessageNew>()
                 .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
                 .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            Mapper.CreateMap<CWeigthScaleMessageOld, WeightScaleMessageOld>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            Mapper.CreateMap<WeightScaleMessageNew, CWeigthScaleMessageNew>()
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            Mapper.CreateMap<WeightScaleMessageOld, CWeigthScaleMessageOld>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            Mapper.CreateMap<CValidationMessage, ValidationMessage>();
            Mapper.CreateMap<ValidationMessage, CValidationMessage>();
            Mapper.CreateMap<IWeightScaleMessage, CWeigthScaleMessageBase>()
                   .Include<WeightScaleMessageOld, CWeigthScaleMessageOld>()
                   .Include<WeightScaleMessageNew, CWeigthScaleMessageNew>();

        }
    }
}
