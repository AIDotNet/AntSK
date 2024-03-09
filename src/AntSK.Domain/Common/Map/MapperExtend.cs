namespace AntSK.Domain.Common.Map
{
    public static class MapperExtend
    {
        /// <summary>
        /// Entity集合转DTO集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> ToDTOList<T>(this object value)
        {
            if (value == null)
                return new List<T>();

            return AutoMapper.Mapper.Map<List<T>>(value);
        }
        /// <summary>
        /// Entity转DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToDTO<T>(this object value)
        {
            if (value == null)
                return default(T);

            return AutoMapper.Mapper.Map<T>(value);
        }
    }
}
