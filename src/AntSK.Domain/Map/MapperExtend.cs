using AutoMapper;

namespace AntSK.Domain.Map
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

            return Mapper.Map<List<T>>(value);
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

            return Mapper.Map<T>(value);
        }

        /// <summary>
        /// 给已有对象map,适合update场景，如需过滤空值需要在AutoMapProfile 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T MapTo<T>(this object self, T result)
        {
            if (self == null)
                return default(T);
            return (T)Mapper.Map(self, result, self.GetType(), typeof(T));
        }
    }
}
