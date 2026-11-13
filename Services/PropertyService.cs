using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryDistrict.Services
{
    public static class PropertyService
    {

        /// <summary>
        /// Рассчитывает индивидуальный коэффициент налога на недвижимость (Property Tax Rate) 
        /// на основе географического положения объекта и плотности его застройки.
        /// </summary>
        /// <remarks>
        /// <para><b>Алгоритм работы метода включает 5 основных этапов:</b></para>
        /// <list type="number">
        /// <item>
        /// <description>
        /// <i>Валидация:</i> Проверка объекта на <see langword="null"/>, а также обязательное наличие 
        /// непустых строк в полях страны (<see cref="Property.country"/>) и города (<see cref="Property.city"/>).
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <i>Обработка неопределенных (Null) значений:</i> 
        /// Если общая площадь не указана или меньше/равна 0, метод прекращает дальнейшие вычисления 
        /// и возвращает минимальную базовую ставку 1% (0.01). Если количество зданий отрицательное — выбрасывается исключение.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <i>Расчет плотности застройки:</i> Вычисляется относительный показатель количества строений 
        /// на каждые 1000 квадратных метров площади объекта недвижимости.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <i>Определение базовой географической ставки:</i>
        /// По умолчанию для всего мира применяется ставка 2% (0.02). Для России применяется ставка 3% (0.03), 
        /// а для городов федерального значения (Москва и Санкт-Петербург) действует повышенная ставка 5% (0.05).
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <i>Начисление надбавки за плотность:</i> Если вычисленная плотность застройки превышает 10 зданий на 1000 кв.м., 
        /// к базовой ставке прибавляется 0.5% (0.005). Если плотность превышает 50 зданий — прибавляется 1.5% (0.015).
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="property">Объект недвижимости, содержащий информацию о локации, площади и количестве построек.</param>
        /// <returns>
        /// Десятичное число (<see cref="decimal"/>), представляющее финальную процентную ставку налога 
        /// (например, значение <c>0.065</c> соответствует налоговой ставке 6.5%).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если переданный параметр <paramref name="property"/> равен <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если поля <see cref="Property.country"/> или <see cref="Property.city"/> 
        /// пусты, состоят только из пробелов или равны <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, если поле <see cref="Property.buildings_count"/> имеет отрицательное значение.
        /// </exception>

        public static decimal CalculatePropertyTaxRate(locations location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (string.IsNullOrWhiteSpace(location.country) || string.IsNullOrWhiteSpace(location.city))
                throw new ArgumentException("Страна и город обязательны для заполнения.");

            if (!location.area.HasValue || location.area.Value <= 0)
                return 0.01m;

            if (!location.buildings_count.HasValue || location.buildings_count.Value < 0)
                throw new InvalidOperationException("Количество зданий не может быть отрицательным.");

            decimal buildingDensity = (location.buildings_count.Value * 1000m) / location.area.Value;

            decimal baseRate = 0.02m;

            if (location.country.Trim().ToLower().Equals("россия", StringComparison.OrdinalIgnoreCase))
            {
                if (location.city.Trim().ToLower().Equals("москва", StringComparison.OrdinalIgnoreCase) ||
                    location.city.Trim().ToLower().Equals("санкт-петербург", StringComparison.OrdinalIgnoreCase))
                    baseRate = 0.05m;
                else
                    baseRate = 0.03m;
            }

            if (buildingDensity > 50m)
                baseRate += 0.015m;
            else if (buildingDensity > 10m && buildingDensity <= 50m)
                baseRate += 0.005m;

            return baseRate;
        }
    }

}
