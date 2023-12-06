# Script is used for method 3: srcML
#
# It parses the XML tree to find things like: 
#   object instantiations, method calls, arguments, and parameters.
#
# Found dependencies are extracted and put in CSV file.
#

import xml.etree.ElementTree as ET
import csv
import os

ns = {'src': 'http://www.srcML.org/srcML/src'}

def extract_dependencies(xml_file, csv_file):
    tree = ET.parse(xml_file)
    root = tree.getroot()

    dependencies = set()

    for file in root.findall('.//src:unit', ns):
        from_file = file.get('filename')
        imports = {imp.split('.')[-1]: imp for imp in [''.join(name.itertext()) for name in file.findall('.//src:import/src:name', ns)] if 'org.apache' in imp}

        for decl in file.findall('.//src:decl_stmt/src:name', ns):
            if decl.text and decl.text in imports:
                dependencies.add((from_file, imports[decl.text] + '.java'))

            elif decl.text and os.path.exists(os.path.join(os.path.dirname(from_file), decl.text + '.java')):
                dependencies.add((from_file, os.path.join(os.path.dirname(from_file), decl.text + '.java')))

        for decl_stmt in file.findall('.//src:decl_stmt/src:decl/src:type/src:name', ns):
            if decl_stmt.text and decl_stmt.text in imports:
                dependencies.add((from_file, imports[decl_stmt.text] + '.java'))
            elif decl_stmt.text and os.path.exists(os.path.join(os.path.dirname(from_file), decl_stmt.text + '.java')):
                dependencies.add((from_file, os.path.join(os.path.dirname(from_file), decl_stmt.text + '.java')))

        for call in file.findall('.//src:call/src:name/src:name', ns):
            if call.text and call.text in imports:
                dependencies.add((from_file, imports[call.text] + '.java'))
            elif call.text and os.path.exists(os.path.join(os.path.dirname(from_file), call.text + '.java')):
                dependencies.add((from_file, os.path.join(os.path.dirname(from_file), call.text + '.java')))

        for argument in file.findall('.//src:argument/src:expr/src:name/src:name', ns):
            if argument.text and argument.text in imports:
                dependencies.add((from_file, imports[argument.text] + '.java'))
            elif argument.text and os.path.exists(os.path.join(os.path.dirname(from_file), argument.text + '.java')):
                dependencies.add((from_file, os.path.join(os.path.dirname(from_file), argument.text + '.java')))                

        for parameter in file.findall('.//src:parameter/src:decl/src:type/src:name', ns):
            if parameter.text and parameter.text in imports:
                dependencies.add((from_file, imports[parameter.text] + '.java'))
            elif parameter.text and os.path.exists(os.path.join(os.path.dirname(from_file), parameter.text + '.java')):
                dependencies.add((from_file, os.path.join(os.path.dirname(from_file), parameter.text + '.java')))  

    # Write to CSV
    with open(csv_file, 'w', newline='') as f:
        writer = csv.writer(f)
        writer.writerow(['From File', 'To File'])
        writer.writerows(dependencies)

# Usage
extract_dependencies('flink.xml', 'flink_dependency.csv')





