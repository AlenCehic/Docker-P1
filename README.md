# Zusammenfassung P1 Docker

## Lernziele:
- Sie können die Begriffe "Containerisierung", "Image", "Layer", "Container", "Repository", "Registry" und Dockerfile erklären
- Sie können die Begriffe Virtualisierung und Cloud voneinander trennen
- Sie kennen die elementaren Commands, um Images und Container zu erzeugen, starten, beenden und löschen
- Sie können die wichtigsten Optionen von docker run (--name, --rm, --network, --ip, -d, -it, -p, -v, -e) korrekt anwenden
- Sie können verschiedene Versionen (tags) eines COntainer-Images nutzen
- Sie können Portweiterleitungen in Betrieb nehmen
- Sie können benannte und gemountete Volumen korrekt einsetzen
- Sie verstehen den Standardaufbau eines Netzwerks mit Docker
- Sie können Docker-Netzwerke definieren und diese den Containern zuweisen
- Sie kennen die Syntax von Dockerfiles
- Sie können die 12 verschiedenen Anweisungen von Dockerfiles korrekt anwenden
- Sie können zwischen ENTRXPOINT und CMD sowie COPY und ADD unterscheiden
- Sie können eigene Dockerfiles für eine Webseite erstellen, testen und dokumentieren

# Begriffe
- Containerisierung
	> Containervirtualisierung (oder Containering) ist eine Methode, um mehrere Instanzen eines Betriebssystems (als "Gäste") isoliert voneinander den Kernel eines Hostsystems nutzen zu lassen.
- Image
	> Ein Image enthält alle Benötigten Komponenten, inkl. Bibliotheken, Hilfsprogramme und sonstige Dateien für den Betrieb.
- Layer
- Container
	> Ein Image bleibt nach dem Download immer unverändert. Das Image ist quasi schreibgeschützt. Ein Image kann auch nicht gestartet werden, sondern es wird aus dem Image heraus ein (oder mehrere) Container gestartet. Schreiboperationen des Containers landen in einem Overlay-Dateisystem, welches "über" dem Dateisystem des Images liegt.
- Repository
- Registry
- Dockerfile

# Virtualisierung vs Cloud

# Elementare Commands
- docker build
- docker push
- docker rmi
- docker pull
- docker run

| Option            | Description                                                                                                     |
| ----------------- | :-------------------------------------------------------------------------------------------------------------- |
| -it               | steht für interaktiv, es wird eine Shell innerhalb des Containers geöffnet                                      |
| --name            | gibt den Namen des Containers an                                                                                |
| --detach, -d      | Führt den Container im Hintergrund aus. Gibt beim Start die Container-ID in der Konsole aus                     |
| --interactive, -i | Lässt STDIN (Standard Input) geöffnet, auch wenn der Container im Hintergrund ausgeführt wird                   |
| --publish, -p     | Veröffentlicht den Port eines Containers für den Host z.B. -p 80:8080 mappt Container-Port 8080 zu Host-Port 80 |
| --tty, -t         | Ordnet ein Pseudo-TTY (Pseudo-Terminal) zu                                                                      |
| --rm              | entfernt den Container bei Programmende                                                                         |

- docker start
- docker stop
- docker rm
- docker ps
- - -a gibt ein liste aller vorhandenen container (auch die die gestoppt sind) ansonsten werden nur laufende container angezeigt
- docker exec
  
# Versionen (tags) eines Container-Images

# Portweiterleitungen
- -p
  
# Volumes
Hier geht es darum, wo ein Container seine Daten speichert. Das Docker-Zustandsdiagramm legt nahe, dass Daten die in einem laufenden Container gespeichert sind, enthalten bleiben, wenn der Container gestoppt und wieder gestartet wird, nicht jedoch wenn ein Container gelöscht und wieder neu erzeugt wird.

**Unbenannte Volumes**

**Anonymous Volumes:** Mit folgenden Komanndo kann ein Container aus einem heruntergeladenen Image gestartet werden:
> docker run -d --name mariadb-test -e MYSQL_ROOT_PASSWORD=geheim mariadb

Um herauszufinden wo nun die Daten aus /var/lib/mysql gelandet sind:
> docker inspect -f '{{.Mounts}}' mariadb-test

**Benannte Volumes**

**Named Volumes** Volumes beim Erstellen eines Containers zu bennenen
> docker run -d --name mariadb-test2 -v myvolume:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=geheim mariadb

Gewählter Name für das Unterverzeichnis wird verwendet, Syntax:
> -v volumenname:containerverzeichnis

Volumes in eigenen Verzeichnissen:
> mkdir /home/vmadmin/database
> docker run -d --name mariadb-test3 -v /home/vmadmin/database:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=geheim mariadb

# Docker-Netzwerke
> docker network ls

> ip addr

> docker network inspect

> docker network create \\
> \-\- driver=bridge \\
> \-\- subnet=10.10.10.0/24 \\
> \-\-gateway=10.10.10.1 \\
> my_net

# Syntax von Dockerfiles

# 12 Anweisungen von Dockerfiles
| Schlüsselwort | Bedeutung |
| --- | :--- |
| ADD | Kopiert Dateien vom **Host** oder einer **entfernten URL** in das Dateisystem des Images |
| CMD | Setzt das Kommando, das beim Start des Containers ausgeführt wird, sofern es **nicht übersteuert** wird |
| COPY | Kopiert Dateien vom **Host** in das Dateisystem des Images |
| ENTRYPOINT | Setzt das Kommando, das beim Start des Containers **immer** ausgeführt wird|
| ENV | Setzt eine Umgebungsvariable |
| EXPOSE | Definiert die zur Laufzeit des Containers aktiven Netzwerk-Ports |
| FROM | Setzt das Basis-Image für die nachfolgenden Anweisungen |
| LABEL | Fügt dem Image Metadaten hinzu |
| RUN | Führt das angegebene Kommando einmalig während **docker build** aus und erzeugt dadurch ein neuen **Image-Layer** |
| USER | Gibt den Account für RUN, CMD und ENTRYPOINT an |
| VOLUME | Definiert einen Mount Point auf ein Verzeichnis auf dem Host oder einem anderen Container |
| WORKDIR | Legt das Arbeitsverzeichnis für Run, CMD, COPY etc. fest |

# ENTRYPOINT und CMD
Beide Kommandos geben an, welches Programm beim Start eines Containers mit **docker run** oder **docker start** ausgeführt wird.
Als erstes wird in eckigen Klammern (Array) und doppelten Hochkommas der vollständige Pfad eines Programmes angegeben.
Die anschliessenden Arrayelemente dienen als Parameter für das Kommando
> CMD ["/bin/ls", "/var"]

Bei *CMD* wird ein Kommando, das bei docker run mitgegeben wird, anstelle von CMD ausgeführt
Bei *ENTRYPOINT* wird ein Kommando, das bei docker run mitgegeben wird, als weiterer Parameter zu ENTRXPOINT hinzugefügt

Es können auch beide Schlüsselwörter verwendet werden. Beispielsweise gibt es beim mariadb Image die folgenden Einträge:
> CMD ["mysqld"] <br>
> ENTRYPOINT ["docker-entrypoint.sh"]

# COPY und ADD
Beide Kommandos kopieren Dateien oder Verzeichnisse vom Host in das Imagedateisystem:
> COPY samplesite/ /var/www/html

> ADD myfonts.tgz /usr/local/share/texmf

Im Unterschied zu *COPY* kann *ADD* zusätzlich auch:
- eine URL als Quelle verwenden
- Archivdateien (tar, gz, ..) automatisch entpacken

Mit --chown=user:group kann der Eigentümer und die Gruppe im Zielsystem festgelegt werden
> COPY --chown=node package.json package-lock.json /src/

# Eigenes Dockerfile für Webseite
