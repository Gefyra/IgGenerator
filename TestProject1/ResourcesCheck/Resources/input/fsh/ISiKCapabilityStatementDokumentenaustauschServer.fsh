Instance: ISiKCapabilityStatementDokumentenaustauschServer
InstanceOf: CapabilityStatement
Usage: #definition
* description = 
  "Dieses CapabilityStatement beschreibt alle Interaktionen 
  die ein ISiK-konformes System unterstützen MUSS bzw. KANN,
  um das Bestätigungsverfahren für dieses Modul zu bestehen.  

  Jede Instanz eines bestätigungsrelevanten Systems MUSS an ihrem Endpunkt eine CapabilityStatement-Ressource bereitstellen.
Hierzu MUSS die [capabilities-Interaktion gemäß FHIR-Kernspezifikation](https://hl7.org/fhir/R4/http.html#capabilities) unterstützt werden. 
Der `MODE`-Parameter kann ignoriert werden.  
Das CapabilityStatement in dieser Spezifikation stellt die Anforderungen seitens der gematik dar (`kind = requirements`). 
Zur Unterscheidung von Anforderungen, die erfüllt werden MÜSSEN gegenüber jenen, die erfüllt werden KÖNNEN, 
wird die [CapabilityStatement-Expectation-Extension](https://hl7.org/fhir/R4/extension-capabilitystatement-expectation.html) mit den möglichen Werten `SHALL` (=MUSS) und `MAY` (=KANN) verwendet.  

Eine Server-Instanz MUSS ihrerseits ein CapabilityStatement vom `kind = instance` liefern und im Element `software` den Namen 
und die Versionsnummer angeben. 
Darüber hinaus MUSS in `CapabilityStatement.instantiates` die Canonical URL des nachfolgenden CapabilityStatements angegeben werden.  

Das CapabilityStatement der Instanz MUSS alle Funktionalitäten auflisten, die im folgenden CapabilityStatement mit `SHALL` gekennzeichnet sind. 
Das CapabilityStatement KANN darüber hinaus die mit `MAY` gekennzeichneten Funktionalitäten, sowie weitere Funktionalitäten auflisten, 
sofern diese in der Instanz implementiert wurden.  

Die Verwendung der CapabilityStatement-Expectation-Extension ist im CapabilityStatement der Server-Instanz nicht erforderlich."
* insert Meta-Inst
* insert Meta-CapabilityStatement
* name = "ISiKCapabilityStatementDokumentenaustauschServer"
* title = "ISiK CapabilityStatement Dokumentenaustausch Server"
* contact.telecom.system = #url
* contact.telecom.value = "https://www.gematik.de"
* jurisdiction = urn:iso:std:iso:3166#DE "Germany"
* kind = #requirements
* fhirVersion = #4.0.1
* format[0] = #application/fhir+xml
* format[+] = #application/fhir+json
* rest.mode = #server
  
* rest.resource[+]
  * insert Expectation (#SHALL)
  * type = #DocumentReference
  * documentation = "Für die Ressource DocumentReference MUSS die REST-Interaktion 
  `CREATE` implementiert werden, siehe {{pagelink:Dokumentenbereitstellung}}. 
  Für die Ressource DocumentReference MUSS die REST-Interaktion `READ` implementiert werden, 
  siehe {{pagelink:Dokumentenabfrage}}.   
  Die in IHE-MHD geltende Einschränkung, dass Clients bei allen Suchen mindestens 
  die Parameter patient oder patient.identifier sowie status verwenden müssen, gilt nicht. 
  Siehe dazu Kapitel {{pagelink:Kompatibilitaet}}.  
  Folgende Suchparameter sind für das Bestätigungsverfahren relevant, auch in Kombination: "
  * supportedProfile = "https://gematik.de/fhir/isik/StructureDefinition/ISiKDokumentenMetadaten"
    * insert Expectation (#SHALL)
  * interaction[+]
    * insert Expectation (#SHALL)
    * code = #create
  * interaction[+]
    * insert Expectation (#SHALL)
    * code = #read
  * interaction[+]
    * insert Expectation (#SHALL)
    * code = #search-type
  * insert CommonSearchParameters
    
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "status"
    * definition = "http://hl7.org/fhir/SearchParameter/DocumentReference-status"
    * type = #token
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?status=final`    
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#token).  
        Dieser Suchparameter ist für die Umsetzung des IHE MHD Profils für Clients und Server verpflichend."
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "patient"
    * definition = "http://hl7.org/fhir/SearchParameter/DocumentReference-patient"
    * type = #reference
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?patient=Patient/123`
        `GET [base]/DocumentReference?patient.identifier=http://mein-krankenhaus.example/fhir/sid/patienten|1032702` 
        `GET [base]/DocumentReference?patient.identifier=1032702`  
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#reference).  
        Weitere Informationen zur Suche nach verketteten Parametern finden sich in der FHIR-Kernspezifikation - Abschnitt [Chained Parameters](https://hl7.org/fhir/R4/search.html#chaining).
        Dieser Suchparameter ist für die Umsetzung des IHE MHD Profils für Clients und Server verpflichend."
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "type"
    * definition = "http://hl7.org/fhir/SearchParameter/DocumentReference-type"
    * type = #token
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?type=http://dvmd.de/fhir/CodeSystem/kdl|AD010101`    
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#token).  
        Dieser Suchparameter ist für die Umsetzung des IHE MHD Profils für Server verpflichtend."
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "category"
    * definition = "http://hl7.org/fhir/SearchParameter/DocumentReference-category"
    * type = #token
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?category=http://ihe-d.de/CodeSystem/IHEXDSclassCode|BEF`    
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#token).  
        Dieser Suchparameter ist für die Umsetzung des IHE MHD Profils für Server verpflichtend."
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "creation"
    * definition = "http://profiles.ihe.net/ITI/MHD/SearchParameter/DocumentReference-Creation"
    * type = #date
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?creation=2021-11-05`    
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#date).  
        Dieser Suchparameter ist Teil der IHE-MHD-Spezifikation und für die Umsetzung des IHE MHD Profils für Server verpflichtend."
  * searchParam[+]
    * insert Expectation (#SHALL)
    * name = "encounter"
    * definition = "http://hl7.org/fhir/SearchParameter/DocumentReference-encounter"
    * type = #reference
    * documentation = 
        "**Beispiel:**    
        `GET [base]/DocumentReference?encounter=Encounter/123`    
        **Anwendungshinweis:**   
        Weitere Details siehe [FHIR-Kernspezifikation](https://hl7.org/fhir/R4/search.html#reference).  "
  * searchInclude[+] = "DocumentReference:patient" 
    * insert Expectation(#SHALL)
  * searchInclude[+] = "DocumentReference:encounter"
    * insert Expectation(#SHALL)
  
  * operation[+]
    * insert Expectation (#MAY)
    * name = #update-metadata
    * definition = Canonical(UpdateMetadata)
  * operation[+]
    * insert Expectation (#MAY)
    * name = #generate-metadata
    * definition = "https://profiles.ihe.net/ITI/MHD/OperationDefinition/generate-metadata"


* rest.resource[+]
  * insert Expectation (#SHALL)
  * type = #Binary
  * supportedProfile = "https://gematik.de/fhir/isik/StructureDefinition/ISiKBinary"
    * insert Expectation (#SHALL)
  * interaction[+]
    * insert Expectation (#SHALL)
    * code = #read

