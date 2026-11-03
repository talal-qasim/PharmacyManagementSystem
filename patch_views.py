import os
import re

directory = r'd:\C#\PharmacyManagementSystem\PharmacyApp\Views'

for filename in os.listdir(directory):
    if filename.endswith('.xaml') and filename not in ['MainWindow.xaml', 'SaleCartView.xaml']:
        filepath = os.path.join(directory, filename)
        
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            
        content = re.sub(r'<RowDefinition Height="\*"/>', '<RowDefinition Height="Auto"/>', content)
        
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
