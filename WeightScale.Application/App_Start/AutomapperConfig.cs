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
        private const int CACHE_TEXT_MAXLEN = 320;

        public static void AutoMapperConfig()
        {
            Mapper.CreateMap<CWeigthScaleMessageNew, WeightScaleMessageNew>()
                 .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                 .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                 .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToSByte(src.Number)));
            Mapper.CreateMap<CWeigthScaleMessageOld, WeightScaleMessageOld>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToSByte(src.Number)));
            Mapper.CreateMap<WeightScaleMessageNew, CWeigthScaleMessageNew>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToInt64(src.Number)));
            Mapper.CreateMap<WeightScaleMessageOld, CWeigthScaleMessageOld>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToInt64(src.Number)));
            //Mapper.CreateMap<WeightScaleMessageNew, CWeigthScaleMessageNew>()
            //.ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
            //    .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            //Mapper.CreateMap<WeightScaleMessageOld, CWeigthScaleMessageOld>()
            //    .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int)src.Direction))
            //    .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int)src.MeasurementStatus));
            Mapper.CreateMap<CWeigthScaleMessageNewOverFlow, WeightScaleMessageNewOverFlow>()
                 .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                 .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                 .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToSByte(src.Number)));
            Mapper.CreateMap<WeightScaleMessageNewOverFlow, CWeigthScaleMessageNewOverFlow>()
                 .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => (int) src.Direction))
                 .ForMember(dest => dest.MeasurementStatus, opt => opt.MapFrom(src => (int) src.MeasurementStatus))
                 .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToInt64(src.Number)));

            Mapper.CreateMap<CValidationMessage, ValidationMessage>()
                .ForMember(des => des.Type, opt => opt.MapFrom(src => (int) src.Type));
            Mapper.CreateMap<ValidationMessage, CValidationMessage>()
                  .ForMember(des => des.Text, opt => opt.MapFrom(src => src.Text.Length > CACHE_TEXT_MAXLEN ? src.Text.Substring(0, CACHE_TEXT_MAXLEN) : src.Text))
                  .ForMember(des => des.Type, opt => opt.MapFrom(src => (int) src.Type));
            //Mapper.CreateMap<ValidationMessageCollection, CValidationMessage[]>();
            Mapper.CreateMap<IWeightScaleMessage, CWeigthScaleMessageBase>()
                .Include<WeightScaleMessageOld, CWeigthScaleMessageOld>()
                .Include<WeightScaleMessageNew, CWeigthScaleMessageNew>()
                .Include<WeightScaleMessageNewOverFlow, CWeigthScaleMessageNewOverFlow>();
            Mapper.CreateMap<long, int>().ConvertUsing(Convert.ToInt32);
            Mapper.CreateMap<int, long>().ConvertUsing(Convert.ToInt64);
            Mapper.CreateMap<long, byte>().ConvertUsing(Convert.ToByte);
            Mapper.CreateMap<byte, long>().ConvertUsing(Convert.ToInt64);

        }
    }
}
