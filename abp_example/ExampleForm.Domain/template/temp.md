                {%- if field.class_attributes != null -%}
                    {%- for attr in field.class_attributes -%}
                        {%- if attr.name == "PasteSearch" -%}
x.{{ field.name }}.Contains(input.word)
                        {%- endif -%}
                    {%- endfor -%}
                {%- endif -%}