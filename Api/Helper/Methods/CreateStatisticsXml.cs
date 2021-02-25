using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Api.Dtos;
using Api.Entities;
using ClosedXML.Excel;

namespace Api.Helper.Methods
{
    public static class CreateStatisticsXml
    {
        public static XLWorkbook CreateYearStatistics(IEnumerable<Statistic> statistics, int year)
        {
            var catchStatistikList = new List<CatchStatistic>();
            foreach (var statistic in statistics)
            {
                var xml = new XmlDocument();
                xml.LoadXml(statistic.StatisticXml);
                var newStatistic = new CatchStatistic
                {
                    FishingClub = xml.SelectSingleNode("Statistik/Fischerverein")?.InnerText,
                    Year = xml.SelectSingleNode("Statistik/Jahr")?.InnerText,
                    FirstName = xml.SelectSingleNode("Statistik/Vorname")?.InnerText,
                    LastName = xml.SelectSingleNode("Statistik/Nachname")?.InnerText,
                    Months = new List<Months>()
                };

                var months = xml.SelectNodes("Statistik/Monate");
                if (months != null)
                    foreach (XmlNode month in months)
                    {
                        if (month.HasChildNodes == false) continue;
                        var newMonth = new Months
                        {
                            Month = month.SelectSingleNode("Monat")?.InnerText, Days = new List<Days>()
                        };
                        newStatistic.Months.Add(newMonth);

                        var days = month.SelectNodes("Tage");
                        if (days == null) continue;
                        foreach (XmlNode day in days)
                        {
                            if (day.HasChildNodes == false) continue;
                            var newTag = new Days
                            {
                                Day = day.SelectSingleNode("Tag")?.InnerText,
                                Hour = day.SelectSingleNode("Stunden")?.InnerText,
                                FishCatches = new List<FishCatch>()
                            };
                            newMonth.Days.Add(newTag);

                            var fishCatches = day.SelectNodes("Fang");
                            if (fishCatches == null) continue;
                            foreach (XmlNode fishCatch in fishCatches)
                            {
                                if (fishCatch.HasChildNodes == false) continue;
                                var newFang = new FishCatch
                                {
                                    Number = fishCatch.SelectSingleNode("Anzahl")?.InnerText,
                                    Fish = fishCatch.SelectSingleNode("Fisch")?.InnerText
                                };
                                newTag.FishCatches.Add(newFang);
                            }
                        }
                    }

                catchStatistikList.Add(newStatistic);
            }


            var monthHourDictionary = new Dictionary<int, double>();
            var monthHourFishDictionary = new Dictionary<int, Dictionary<string, int>>();
            foreach (var statistic in catchStatistikList)
            foreach (var month in statistic.Months)
            {
                var check = monthHourDictionary.ContainsKey(int.Parse(month.Month));
                if (check)
                {
                    monthHourDictionary[int.Parse(month.Month)] += month.Days.Sum(a => Convert.ToDouble(a.Hour));
                }
                else
                {
                    monthHourDictionary.Add(int.Parse(month.Month), month.Days.Sum(a => Convert.ToDouble(a.Hour)));
                    monthHourFishDictionary.Add(int.Parse(month.Month), new Dictionary<string, int>());
                }

                foreach (var day in month.Days)
                foreach (var fish in day.FishCatches)
                {
                    if (string.IsNullOrEmpty(fish.Fish)) continue;
                    var checkFish = monthHourFishDictionary[int.Parse(month.Month)].ContainsKey(fish.Fish);
                    if (checkFish)
                        monthHourFishDictionary[int.Parse(month.Month)][fish.Fish] += int.Parse(fish.Number);
                    else
                        monthHourFishDictionary[int.Parse(month.Month)].Add(fish.Fish, int.Parse(fish.Number));
                }
            }

            var l = monthHourDictionary.OrderBy(key => key.Key);
            var orderedDictionary = l.ToDictionary(keyItem => keyItem.Key, valueItem => valueItem.Value);

            var fishNumberDictionary = new Dictionary<string, int>();
            foreach (var statistic in catchStatistikList)
            foreach (var month in statistic.Months)
            foreach (var days in month.Days)
            foreach (var fish in days.FishCatches)
            {
                if (string.IsNullOrEmpty(fish.Fish)) continue;
                var check = fishNumberDictionary.ContainsKey(fish.Fish);
                if (check)
                    fishNumberDictionary[fish.Fish] += int.Parse(fish.Number);
                else
                    fishNumberDictionary.Add(fish.Fish, int.Parse(fish.Number));
            }

            var clubName = "";
            if (catchStatistikList.Count > 0) clubName = catchStatistikList[0].FishingClub;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"Statistik {year}");
            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            var header = worksheet.Cell(1, 1);
            header.Value = $"Statistik {clubName} {year}";
            header.Style.Font.FontSize = 18;
            header.Style.Font.SetBold();
            worksheet.Cell(2, 1).Value = "";
            worksheet.Cell(2, 1).WorksheetRow().Height = 30;
            worksheet.Column("A").Width = 50;
            worksheet.Column("B").Width = 20;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Rows().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var row = 3;

            foreach (var (month, hour) in orderedDictionary)
            {
                worksheet.Cell(row, 1).Value = "Monat";
                worksheet.Cell(row, 1).Style.Font.SetBold();

                worksheet.Cell(row, 2).Value = "Ausgeübte Stunden";
                worksheet.Cell(row, 2).Style.Font.SetBold();
                worksheet.Row(row).Style.Font.FontSize = 12;
                worksheet.Cell(row, 3).Value = "Fischart";
                worksheet.Cell(row, 3).Style.Font.SetBold();
                worksheet.Cell(row, 4).Value = "Anzahl";
                worksheet.Cell(row, 4).Style.Font.SetBold();
                worksheet.Row(row).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                row++;

                worksheet.Cell(row, 1).Style.Font.SetFontSize(12);
                worksheet.Cell(row, 1).Value = GetMonthName(month);
                worksheet.Cell(row, 2).Style.Font.SetFontSize(12);
                worksheet.Cell(row, 2).Value = hour;

                foreach (var (fish, number) in monthHourFishDictionary[month])
                {
                    worksheet.Cell(row, 3).Value = fish;
                    worksheet.Cell(row, 4).Value = number;
                    row++;
                }

                worksheet.Cell(row, 1).WorksheetRow().Height = 20;
                worksheet.Cell(row, 2).WorksheetRow().Height = 20;
                row++;
            }


            worksheet.Cell(++row, 1).Value = "Jahres Total";
            worksheet.Cell(row, 1).Style.Font.SetFontSize(16);
            worksheet.Cell(row, 1).Style.Font.SetBold();
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;
            worksheet.Cell(row, 1).Value = "Stunden";
            worksheet.Cell(row, 1).Style.Font.SetFontSize(12);
            worksheet.Cell(row, 1).Style.Font.SetBold();

            worksheet.Cell(row, 2).Value = orderedDictionary.Sum(a => a.Value);
            worksheet.Cell(row, 2).Style.Font.SetBold();
            row++;
            row++;
            foreach (var (fish, number) in fishNumberDictionary)
            {
                worksheet.Cell(row, 1).Style.Font.SetFontSize(12);
                worksheet.Cell(row, 1).Style.Font.SetBold();
                worksheet.Cell(row, 1).Value = fish;
                worksheet.Cell(row, 2).Style.Font.SetFontSize(12);
                worksheet.Cell(row, 2).Style.Font.SetBold();
                worksheet.Cell(row, 2).Value = number;
                row++;
            }

            return workbook;
        }

        private static string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Januar",
                2 => "Februar",
                3 => "März",
                4 => "April",
                5 => "Mai",
                6 => "Juni",
                7 => "Juli",
                8 => "August",
                9 => "September",
                10 => "Oktober",
                11 => "November",
                12 => "Dezember",
                _ => "Unbekannt"
            };
        }
    }
}