using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HtmlAgilityPack;
using PaginacaoView.Models;

namespace PaginacaoView.FileParser;

public static class Parser
{
    private record struct Discipline(string Name, bool Permission);

    private static string ExtractCodigo(this HtmlNode node)
    {
        var codigo = node.GetClasses().FirstOrDefault(cl => cl.Contains("turmas"));
        if (codigo == null) throw new Exception("CLASS ERROR");
        var index = codigo.IndexOf("turmas", StringComparison.Ordinal) + "turmas".Length;
        return codigo.Substring(index);
    }

    private static IEnumerable<string> ExtractHorarios(this HtmlNode node)
    {
        var label = node.SelectNodes($"{node.XPath}//label").ToList()[2].InnerText;
        if (label?.Contains("(") ?? false)
            label = label.Substring(0, label.IndexOf("(", StringComparison.Ordinal));
        label = label?.Trim();

        return label?.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()) ?? Array.Empty<string>();
    }

    private static DayHour[] ToDayHour(this IEnumerable<string> values)
    {
        var dayHours = new List<DayHour>();

        foreach (var value in values)
        {
            if (value.Contains("T"))
            {
                var splited = value.Split('T');
                var diasString = splited[0];
                var h = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(splited[1][0].ToString())/2));

                dayHours.AddRange(diasString.Select(diaString => Convert.ToInt32(diaString.ToString())).Select(dia => new DayHour { Day = dia, Hour = h }));
            }
            else
            {
                var splited = value.Split('N');
                var diasString = splited[0];
                var h = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(splited[1][0].ToString())/2) + 3);

                dayHours.AddRange(diasString.Select(diaString => Convert.ToInt32(diaString.ToString())).Select(dia => new DayHour { Day = dia, Hour = h }));
            }
        }
        
        return dayHours.ToArray();
    }
    public static List<Turma> Parse(string path)
    {
        var doc = new HtmlDocument();
        doc.Load(path);
        var listaTurmas = doc.DocumentNode.SelectSingleNode("//*[@id='lista-turmas-curriculo']");

        var turmas = new List<Turma>();
        var disciplines = new Dictionary<string, Discipline>();
        foreach (var row in listaTurmas.SelectNodes("//tr"))
        {
            if (row.HasClass("linhaPar") || row.HasClass("linhaImpar"))
            {
                var codigo = row.ExtractCodigo();
                var (name, permission) = disciplines[codigo];
                var horarios = row.ExtractHorarios().ToDayHour();
                turmas.Add(new Turma()
                {
                    Name = name,
                    Permission = permission,
                    DayHour = horarios
                });
            }
            else if (row.HasClass("disciplina"))
            {
                var tag = row.SelectSingleNode($"{row.XPath}//a");
                var nome = tag.InnerText;
                nome = nome.Substring(nome.IndexOf('-') + 1).Trim();

                var codigo = tag.Attributes["onclick"].Value;
                var index = codigo.IndexOf('(') + 1;
                var length = codigo.IndexOf(',') - index;
                codigo = codigo.Substring( index, length);

                var img = row.SelectSingleNode($"{row.XPath}//img");
                var permission = img.Attributes.FirstOrDefault(a => a.Name == "alt") != null;

                disciplines[codigo] = new Discipline(nome, permission);
            }

        }
        return turmas;
    }
}