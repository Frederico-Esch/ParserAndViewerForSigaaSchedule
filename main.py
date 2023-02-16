from bs4 import BeautifulSoup as BS
from math import ceil
import json

def read_file():
    with open("index.html", "r") as file:
        return file.read()

def translate_turma(nomes, turma):
    c = ""
    for cl in turma.attrs["class"]:
        if "turma" in cl:
            c : str = cl
            break
    c = c.replace("turmas", "")
    return nomes[c]

def extract_name_and_code(turma):
    tag = turma.find_all("a")[0]
    text = tag.attrs["onclick"]
    b = text.index("(")
    e = text.index(",")
    codigo = text[b+1:e]

    nome = tag.text
    b = nome.index("-")
    nome = nome[b+1:].strip()
    return nome, codigo

def extrat_permission(turma):
    img = turma.find_all("img")[0]
    return "alt" in img.attrs.keys()

def adapt_horarios(horarios):
    correct = []
    for horario in horarios:
        if horario.find("T") >= 0:
            t = horario.find("T")
            dias = horario[:t]
            inicio = int(horario[t+1])
            inicio = ceil(inicio/2)
            for dia in dias:
                h = { "Day": int(dia), "Hour": inicio }
                correct.append(h)
        else:
            n = horario.find("N")
            dias = horario[:n]
            inicio = int(horario[n+1])+6
            inicio = ceil(inicio/2)
            for dia in dias:
                h = { "Day": int(dia), "Hour": inicio }
                correct.append(h)
    return correct

def adapt_turma(turma, disciplinas):
    horarioLable = turma.find_all("label")[2].text
    e = horarioLable.index("(")
    horarioLable = horarioLable[:e].strip()
    horarios = horarioLable.split()
    obj = translate_turma(disciplinas, turma)
    horarios = adapt_horarios(horarios)
    return {
        "Name": obj["nome"],
        "Permission": obj["permissao"],
        "DayHour": horarios
    }

data = read_file()
data = BS(data, features="html.parser")
#print(data)


lista_de_turmas = data.find_all(name = None, attrs= {
    "id": "lista-turmas-curriculo"
})[0]

disciplinas = {}
turmas = []
for row in lista_de_turmas.find_all("tr"):
    if "class" not in row.attrs.keys():
        continue

    classes = row.attrs["class"]
    if "linhaPar" in classes or "linhaImpar" in classes:
        turmas.append(adapt_turma(row, disciplinas))
    elif "disciplina" in classes:
        nome, codigo = extract_name_and_code(row)
        permission = extrat_permission(row)
        disciplinas[codigo] = {
            "nome": nome,
            "permissao": permission
        }

data = json.dumps(turmas, indent=2)
with open("output.json", "w") as file:
    file.write(data)
