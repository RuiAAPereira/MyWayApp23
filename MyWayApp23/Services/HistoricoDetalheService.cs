﻿namespace MyWayApp23.Services;

public class HistoricoDetalheService : IHistoricoDetalheService
{
    private readonly DataContext _context;

    public HistoricoDetalheService(DataContext context)
    {
        _context = context;
    }

    public List<HistoricoDetalhe> GetDetalhes(DateTime data)
    {
        List<string> _mangas = TipoStand("JETBRIDGE");
        List<string> _remotos = TipoStand("REMOTE");
        List<HistoricoDetalhe> detalhes = new();

        var historico = _context.HistoricoAssistencias!.ToList();

        foreach (DateTime date in DateHelper.AllDatesInMonth(data.Year, data.Month))
        {
            List<HistoricoAssistencia>? detalhesData = historico.Where(d => d.Data.Date == date.Date).ToList();

            TimeOnly average = TimeOnly.FromTimeSpan(new TimeSpan(0));

            List<TimeSpan> media = new();
            foreach (var row in detalhesData.ToList())
            {
                TimeSpan fim = new();
                if (row.Fim != null) { fim = row.Fim.Value.TimeOfDay; }
                TimeSpan inicio = new();
                if (row.Inicio != null) { inicio = row.Inicio.Value.TimeOfDay; }

                if (fim > inicio)
                {
                    TimeSpan m = fim - inicio;
                    media.Add(m);
                }
            }

            if (media.Count > 0)
            {
                double doubleAverageTicks = media.Average(timeSpan => timeSpan.Ticks);
                long longAverageTicks = Convert.ToInt64(doubleAverageTicks);

                average = TimeOnly.FromTimeSpan(new TimeSpan(longAverageTicks));
            }

            HistoricoDetalhe detalhe = new()
            {
                Data = DateOnly.FromDateTime(date),
                DiaSemana = date.ToString("ddd", CultureInfo.CreateSpecificCulture("pt-PT")),
                TotalDia = detalhesData.Count,
                Dep = detalhesData.Count(m => m.Mov == "D"),
                Arr = detalhesData.Count(m => m.Mov == "A"),
                JetBridge = detalhesData.Count(m => _mangas.Contains(m.Stand!)),
                Remote = detalhesData.Count(m => _remotos.Contains(m.Stand!)),
                Mais36 = detalhesData.Count(m => m.Notif >= 36),
                Menos36 = detalhesData.Count(m => m.Notif < 36),
                Wchr = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wchr")),
                Wchs = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wchs")),
                Wchc = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wchc")),
                Maas = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("maas")),
                Blnd = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("blnd")),
                Deaf = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("deaf")),
                Dpna = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("dpna")),
                Stcr = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("stcr")),
                Meda = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("meda")),
                Wcmp = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wcmp")),
                Wcbd = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wcbd")),
                Wcbw = detalhesData.Count(m => m.SSR.ContainsCaseInsensitive("wcbw")),
                Media = average
            };

            if (detalhesData.Count > 0)
            {
                detalhe.DepPercentage = PercentageHelper.PercentageToString(detalhesData.Count(m => m.Mov == "D"), detalhesData.Count);
                detalhe.ArrPercentage = PercentageHelper.PercentageToString(detalhesData.Count(m => m.Mov == "A"), detalhesData.Count);
                detalhe.JetBridgePercentage = PercentageHelper.PercentageToString(detalhesData.Count(m => _mangas.Contains(m.Stand!)), detalhesData.Count);
                detalhe.RemotePercentage = PercentageHelper.PercentageToString(detalhesData.Count(m => _remotos.Contains(m.Stand!)), detalhesData.Count);
                detalhe.PercentageMais36 = PercentageHelper.PercentageToString(detalhesData.Count(m => m.Notif >= 36), detalhesData.Count);
                detalhe.PercentageMenos36 = PercentageHelper.PercentageToString(detalhesData.Count(m => m.Notif < 36), detalhesData.Count);
            }

            detalhes.Add(detalhe);
        }

        return detalhes.Where(t => t.TotalDia > 0).ToList();
    }

    private List<string> TipoStand(string tipo)
    {
        return _context.Stands!.Where(t => t.Tipo == tipo).Select(m => m.Numero).ToList();
    }

    //private List<TimeSpan> TimeSpanFromDates(DateTime? i, DateTime? f)
    //{
    //    List<TimeSpan> media = new();
    //    TimeSpan fim = new();
    //    if (f != null) { fim = f.Value.TimeOfDay; }
    //    TimeSpan inicio = new();
    //    if (i != null) { inicio = i.Value.TimeOfDay; }

    //    if (fim > inicio)
    //    {
    //        TimeSpan m = fim - inicio;
    //        media.Add(m);
    //    }
    //    else { media.Add(TimeSpan.Zero); }

    //    return media;
    //}
}
