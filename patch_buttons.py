import os
import re

dirs = [r'd:\C#\PharmacyManagementSystem\PharmacyApp\Controls', r'd:\C#\PharmacyManagementSystem\PharmacyApp\Views']

for d in dirs:
    for filename in os.listdir(d):
        if filename.endswith('.xaml'):
            # Skip LoginView as it might have specific styling, SaleCartView might also be fine but let's check
            filepath = os.path.join(d, filename)
            
            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()
            
            # Find all <Button tags that do NOT contain the word Style=
            # Use negative lookahead to ensure there's no Style= between <Button and >
            # We must be careful about line breaks inside the tag!
            # Instead of complex regex, let's just find `<Button ` and check if it has `Style=` before `>`
            
            # Simple approach: find all <Button ...>
            def replacer(match):
                tag = match.group(0)
                if 'Style=' not in tag and 'Click="CloseButton_Click"' not in tag and 'Content="✕"' not in tag:
                    return tag.replace('<Button ', '<Button Style="{StaticResource SecondaryButtonStyle}" ')
                return tag
                
            new_content = re.sub(r'<Button\s[^>]*>', replacer, content, flags=re.DOTALL)
            
            if content != new_content:
                with open(filepath, 'w', encoding='utf-8') as f:
                    f.write(new_content)
